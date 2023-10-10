using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Ajax.Utilities;
using Microsoft.Practices.Unity;
using PagedList;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Product.Controllers
{
    [EsAuthorize(Roles = "ProductsAuthorize, ProductsEdit, ProductsView")]
    public class BOMLineItemController : BaseController
    {
        private readonly IBOMLineItemService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        [Dependency]
        public IBOMService BOMService { get; set; }

        [Dependency]
        public IItemService ItemService { get; set; }

        [Dependency]
        public IProductService ProductService { get; set; }

        [Dependency]
        public IWeightUnitService WeightUnitService { get; set; }

        public BOMLineItemController(IBOMLineItemService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
        }

        // GET: BOMDetl
        public ActionResult Index(string id)
        {
            ViewBag.ProductId = id;

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = _service.SelectLineItems(id, applicationUser.Id, applicationUser.OrgId.ToString());

            return PartialView(model);
        }


        // GET: BOMDetl/Details/5
        public async Task<ActionResult> Details(string productId, string itemId)
        {
            if (itemId == null || productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            BOMLineItem instance = await _service.FindAsync(productId, itemId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.BOM = await BOMService.FindAsync(instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            instance.AspNetUser = Lifetrons.Erp.Helpers.ControllerHelper.GetAspNetUser(instance.CreatedBy);
            instance.AspNetUser1 = Lifetrons.Erp.Helpers.ControllerHelper.GetAspNetUser(instance.ModifiedBy);
            instance.WeightUnit = await WeightUnitService.FindAsync(instance.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.WeightUnit1 = await WeightUnitService.FindAsync(instance.AllowedLossWeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Item = await ItemService.FindAsync(instance.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            ViewBag.ProductName = (await ProductService.FindAsync(productId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(instance);
        }

        // GET: BOMDetl/Create
        [EsAuthorize(Roles = "ProductsAuthorize, ProductsEdit")]
        public async Task<ActionResult> Create(string id)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.ItemId = new SelectList(await ItemService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.WeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.AllowedLossWeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ProductName = (await ProductService.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.ProductId = id;

            var model = InitializeCreateModel(id, applicationUser);

            return View(model);
        }

        private BOMLineItem InitializeCreateModel(string productId, ApplicationUser applicationUser)
        {
            ViewBag.ItemId = new SelectList(ItemService.Select(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            var model = new BOMLineItem
            {
                ProductId = productId.ToSysGuid(),
                ShrtDesc = string.Empty,
                Quantity = 1,
                AllowedLossQuantity = 0,
            };

            return model;
        }

        // POST: BOMDetl/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ProductsAuthorize, ProductsEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Id,ProductId,ItemId,Serial,ShrtDesc,Desc,Quantity,Weight,WeightUnitId,Rate,AllowedLossQuantity,AllowedLossWeight,AllowedLossWeightUnitId,Authorized")] BOMLineItem instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

                instance.Id = Guid.NewGuid();
                instance.OrgId = applicationUser.OrgId.ToSysGuid();
                instance.CreatedBy = applicationUser.Id;
                instance.CreatedDate = DateTime.UtcNow;
                instance.ModifiedBy = applicationUser.Id;
                instance.ModifiedDate = DateTime.UtcNow;
                instance.Active = true;
                //In Create - UnAuthourize instance if used do not has the Authorize role.
                if (instance.Authorized && !User.IsInRole("ProductsAuthorize"))
                {
                    instance.Authorized = false;
                }

                try
                {
                    _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    var exceptionMessage = HandleDbEntityValidationException(ex);
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException exception)
                {
                    HandleDbUpdateException(exception);
                    throw;
                }
            }

            return RedirectToAction("Details", new { productId = instance.ProductId.ToString(), itemId = instance.ItemId.ToString() });
        }

        // GET: BOMDetl/Edit/5
        [EsAuthorize(Roles = "ProductsAuthorize, ProductsEdit")]
        public async Task<ActionResult> Edit(string productId, string itemId)
        {
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(itemId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            BOMLineItem instance = await _service.FindAsync(productId, itemId, applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "ProductsAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("ProductsAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            ViewBag.WeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.WeightUnitId);
            ViewBag.AllowedLossWeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.AllowedLossWeightUnitId);

            ViewBag.ProductId = productId;
            ViewBag.ProductName = (await ProductService.FindAsync(productId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            ViewBag.ItemId = itemId;
            ViewBag.ItemName = (await ItemService.FindAsync(itemId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(instance);
        }

        // POST: BOMDetl/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ProductsAuthorize, ProductsEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProductId,ItemId,Serial,ShrtDesc,Desc,Quantity,Weight,WeightUnitId,Rate,AllowedLossQuantity,AllowedLossWeight,AllowedLossWeightUnitId,Authorized")] BOMLineItem instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                BOMLineItem model = await _service.FindAsync(instance.ProductId.ToString(), instance.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  ProductsAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("ProductsAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Desc = instance.Authorized ? "[Authorized] " + instance.Desc : "[UnAuthorized] " + instance.Desc;
                    }
                }
                //If user is not in role of ProductsAuthorize and record is Authorized, stop editng and show appropriate message.
                else if (model.Authorized)
                {
                    TempData["CustomErrorMessage"] = "Authorized record cannot be edited. Please Un-authorize the record to enable editing." +
                                                     "You should have authorization rights to authorize/unauthorize records.";
                    return RedirectToAction("Error", "Error", new { message = TempData["CustomErrorMessage"] });
                }

                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;
                model.Authorized = instance.Authorized;
                model.ShrtDesc = instance.ShrtDesc;
                model.Desc = instance.Desc;

                model.Serial = instance.Serial;
                model.Quantity = instance.Quantity;
                model.Weight = instance.Weight;
                model.WeightUnitId = instance.WeightUnitId;
                model.AllowedLossQuantity = instance.AllowedLossQuantity;
                model.AllowedLossWeight = instance.AllowedLossWeight;
                model.AllowedLossWeightUnitId = instance.AllowedLossWeightUnitId;
                model.Rate = instance.Rate;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "ItemId", "ShrtDesc", "Desc", "Quantity", "Weight", "WeightUnitId", "Rate", "AllowedLossQuantity", "AllowedLossWeight", "AllowedLossWeightUnitId", "Authorized" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { productId = instance.ProductId.ToString(), itemId = instance.ItemId.ToString(), });
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    var exceptionMessage = HandleDbEntityValidationException(ex);
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException exception)
                {
                    HandleDbUpdateException(exception);
                    throw;
                }

                return View(instance);
            }

            return HttpNotFound();
        }

        // GET: BOMDetl/Delete/5
        [EsAuthorize(Roles = "ProductsAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid productId, Guid itemId)
        {
            if (productId.ToString().IsNullOrWhiteSpace() || itemId.ToString().IsNullOrWhiteSpace())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            BOMLineItem instance = await _service.FindAsync(productId.ToString(), itemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.BOM = await BOMService.FindAsync(instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            instance.AspNetUser = Lifetrons.Erp.Helpers.ControllerHelper.GetAspNetUser(instance.CreatedBy);
            instance.AspNetUser1 = Lifetrons.Erp.Helpers.ControllerHelper.GetAspNetUser(instance.ModifiedBy);
            instance.WeightUnit = await WeightUnitService.FindAsync(instance.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.WeightUnit1 = await WeightUnitService.FindAsync(instance.AllowedLossWeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Item = await ItemService.FindAsync(instance.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            ViewBag.ProductName = (await ProductService.FindAsync(productId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(instance);
        }

        // POST: BOMDetl/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ProductsAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid productId, Guid itemId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    BOMLineItem model = await _service.FindAsync(productId.ToString(), itemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted BOM Line Item: ProductId=" + productId + " ItemId=" + itemId);
                }
                return RedirectToAction("Details", "BOM", new { id = productId });
            }

            return HttpNotFound();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Handle Exception
        private void HandleDbUpdateException(DbUpdateException exception)
        {
            string innerExceptionMessage = exception.InnerException.InnerException.Message;
            TempData["CustomErrorMessage"] = innerExceptionMessage;
            TempData["CustomErrorDetail"] = exception.InnerException.Message;

            if (innerExceptionMessage.Contains("UQ") ||
                innerExceptionMessage.Contains("Primary Key") ||
                innerExceptionMessage.Contains("PK"))
            {
                if (innerExceptionMessage.Contains("PriceBookId_ProductId"))
                {
                    TempData["CustomErrorMessage"] = "Duplicate Product. Product already exists.";
                }
                else
                {
                    TempData["CustomErrorMessage"] = "Duplicate Name or Code. Key record already exists.";
                }
            }
        }

        private string HandleDbEntityValidationException(DbEntityValidationException ex)
        {
            // Retrieve the error messages as a list of strings.
            var errorMessages = ex.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

            // Join the list to a single string.
            var fullErrorMessage = string.Join("; ", errorMessages);

            // Combine the original exception message with the new one.
            var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

            // Throw a new DbEntityValidationException with the improved exception message.
            TempData["CustomErrorMessage"] = exceptionMessage;
            TempData["CustomErrorDetail"] = ex.GetType();
            return exceptionMessage;
        }

        #endregion Handle Exception
    }
}
