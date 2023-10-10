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
using System.Web.WebPages;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using PagedList;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Product.Controllers
{
    [EsAuthorize(Roles = "ProductsAuthorize, ProductsEdit, ProductsView")]
    public class BOMController : BaseController
    {
        private readonly IBOMService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        [Dependency]
        public IItemService ItemService { get; set; }

        [Dependency]
        public IProductService ProductService { get; set; }

        [Dependency]
        public IWeightUnitService WeightUnitService { get; set; }

        public BOMController(IBOMService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
        }

        // GET: BOMHead
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.CreatedDate).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }


        // GET: BOMHead/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            BOM instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return RedirectToAction("Create", new { id = id });
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            instance.AspNetUser2 = ControllerHelper.GetAspNetUser(instance.OwnerId); //Owner

            instance.Product = await ProductService.FindAsync(instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.WeightUnit = await WeightUnitService.FindAsync(instance.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.ProductId = instance.ProductId;
            ViewBag.Name = instance.Name;

            return View(instance);
        }

        // GET: BOMHead/Create
        [EsAuthorize(Roles = "ProductsAuthorize, ProductsEdit")]
        public async Task<ActionResult> Create(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.WeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            ViewBag.ProductName = (await ProductService.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.ProductId = id;

            //Initialize
            BOM model = new BOM
            {
                ProductId = id.ToSysGuid(),
                Name = ViewBag.ProductName,
                Code = ViewBag.ProductName,
            };

            return View(model);
        }

        // POST: BOMHead/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ProductsAuthorize, ProductsEdit")]
        public async Task<ActionResult> Create([Bind(Include = "ProductId,Name,Code,ShrtDesc,Desc,Quantity,Weight,WeightUnitId,OtherCharges,OwnerId")] BOM instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

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

                instance.Desc = instance.Desc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Desc;
                instance.Name = instance.Name + Helper.SysSeparator + DateTime.UtcNow;
                instance.Code = string.IsNullOrEmpty(instance.Code) ? instance.Name : instance.Code + Helper.SysSeparator + DateTime.UtcNow;

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

            return RedirectToAction("Details", new { id = instance.ProductId });
        }

        // GET: BOMHead/Edit/5
        [EsAuthorize(Roles = "ProductsAuthorize, ProductsEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            BOM instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProductName = (await ProductService.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.ProductId = id;

            //If instance.Authorized, only user with "ProductsAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("ProductsAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.OwnerId);
            ViewBag.WeightUnitId = new SelectList(await WeightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.WeightUnitId);

            return View(instance);
        }

        // POST: BOMHead/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ProductsAuthorize, ProductsEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "ProductId,Name,Code,ShrtDesc,Desc,Quantity,Weight,WeightUnitId,OtherCharges,OwnerId,Authorized")] BOM instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                BOM model = await _service.FindAsync(instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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
                model.Name = instance.Name;
                model.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code;
                model.ShrtDesc = instance.ShrtDesc;
                string instanceDesc = instance.Desc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Desc;
                model.Desc = model.Desc.IsEmpty() ? instanceDesc : model.Desc + "\n" + instanceDesc;

                model.OwnerId = instance.OwnerId;
                model.Quantity = instance.Quantity;
                model.Weight = instance.Weight;
                model.WeightUnitId = instance.WeightUnitId;
                model.OtherCharges = instance.OtherCharges;

                ModelState.Clear();

                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Name", "Code", "ShrtDesc", "Desc", "Quantity", "Weight", "WeightUnitId", "OtherCharges", "OwnerId", "Authorized" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { id = model.ProductId });
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

        // GET: BOMHead/Delete/5
        [EsAuthorize(Roles = "ProductsAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            BOM model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: BOMHead/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ProductsAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null)
                {
                    BOM model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    _service.Delete(id.ToString());

                    // Object Comparison and logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "BOM Deleted: ProductId=" + id + " Name=" + model.Name + " Code=" + model.Code);
                }
                return RedirectToAction("Index");
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

        public async Task<ActionResult> Search(string searchParam, int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage =
                 records.Where(x => x.Name.ToLower().Contains(searchParam.ToLower()) || x.Code.ToLower().Contains(searchParam.ToLower()))
                    .OrderByDescending(x => x.ModifiedDate)
                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
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
