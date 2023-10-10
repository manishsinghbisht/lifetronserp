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
using Repository;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Sales.Controllers
{
    [EsAuthorize(Roles = "SalesAuthorize, SalesEdit, SalesView")]
    public class InvoiceLineItemController : BaseController
    {
        private readonly IInvoiceLineItemService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;

        private readonly IInvoiceService _invoiceService;
        private readonly IPriceBookLineItemService _priceBookLineItemService;
        private readonly IWeightUnitService _weightUnitService;

        // GET: /PriceBookLineItem/
        public InvoiceLineItemController(IInvoiceLineItemService service, IUnitOfWorkAsync unitOfWork, IInvoiceService invoiceService,
            IPriceBookService priceBookService, IPriceBookLineItemService priceBookLineItemService, IWeightUnitService weightUnitService)
        {
            _service = service;
            _unitOfWork = unitOfWork;

            //Price Book Services
            _invoiceService = invoiceService;
            _priceBookLineItemService = priceBookLineItemService;
            _weightUnitService = weightUnitService;
        }

        // GET: /InvoiceLineItem/
        public ActionResult Index(string invoiceId)
        {
            ViewBag.InvoiceId = invoiceId;
            ViewBag.PriceBookId = null;

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = _service.SelectLineItems(invoiceId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (model.Any())
            {
                ViewBag.PriceBookId = model.FirstOrDefault().PriceBookId;
            }
            return PartialView(model);
        }

        // GET: /InvoiceLineItem/Details/5
        public async Task<ActionResult> Details(string invoiceId, string priceBookId, string productId)
        {
            if (priceBookId == null || productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var instance = await _service.FindAsync(invoiceId, priceBookId, productId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.Invoice = await _invoiceService.FindAsync(instance.InvoiceId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            instance.AspNetUser = Lifetrons.Erp.Helpers.ControllerHelper.GetAspNetUser(instance.CreatedBy);
            instance.AspNetUser1 = Lifetrons.Erp.Helpers.ControllerHelper.GetAspNetUser(instance.ModifiedBy);
            instance.WeightUnit = await _weightUnitService.FindAsync(instance.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.PriceBookLineItem = await _priceBookLineItemService.FindAsync(instance.PriceBookId.ToString(), instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.PriceBook = instance.PriceBookLineItem.PriceBook;

            return View(instance);
        }

        // GET: /Invoice/Create
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create(string invoiceId, string priceBookId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.InvoiceId = invoiceId;
            ViewBag.InvoiceName = (await _invoiceService.FindAsync(invoiceId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.PriceBookId = null;

            if (!string.IsNullOrEmpty(priceBookId))
            {
                ViewBag.PriceBookId = priceBookId;
            }

            var model = InitializeCreateModel(priceBookId, applicationUser);

            return View(model);
        }

        private InvoiceLineItem InitializeCreateModel(string priceBookId, ApplicationUser applicationUser)
        {
            //Load product from PricBookLineItemSelector
            var model = new InvoiceLineItem();

            if (TempData["PriceBookLineItemSelectorSelectedId"] != null)
            {
                try
                {
                    var selectedProductId = TempData["PriceBookLineItemSelectorSelectedId"].ToString().ToSysGuid();
                    PriceBookLineItem priceBookLineItem = _priceBookLineItemService.Find(priceBookId, selectedProductId.ToString(),
                                applicationUser.Id, applicationUser.OrgId.ToString());
                    model = new InvoiceLineItem()
                    {
                        ProductId = priceBookLineItem.ProductId,
                        PriceBookId = priceBookLineItem.PriceBookId,
                        LineItemPrice = priceBookLineItem.ListPrice,
                        SalesPrice = priceBookLineItem.ListPrice,
                        ShrtDesc = priceBookLineItem.ShrtDesc,
                        Quantity = 1,
                        DiscountPercent = 0,
                    };
                }
                catch (Exception)
                {
                }
                finally
                {
                    TempData["PriceBookLineItemSelectorSelectedId"] = null;
                }
            }
            return model;
        }

        // POST: /InvoiceLineItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create([Bind(Include = "ShrtDesc,Desc,InvoiceId,PriceBookId,ProductId,SalesPrice,Quantity,Weight,WeightUnitId,DiscountPercent,DiscountAmount,LineItemPrice,Serial,SpecialInstructions,Authorized")] InvoiceLineItem instance)
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

            return RedirectToAction("Details", new { invoiceId = instance.InvoiceId.ToString(), priceBookId = instance.PriceBookId.ToString(), productId = instance.ProductId.ToString() });
        }

        // GET: /InvoiceLineItem/Edit/5
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Edit(string invoiceId, string priceBookId, string productId)
        {
            if (string.IsNullOrEmpty(invoiceId) || string.IsNullOrEmpty(priceBookId) || string.IsNullOrEmpty(productId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            InvoiceLineItem instance = await _service.FindAsync(invoiceId, priceBookId, productId, applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "SalesAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("SalesAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.WeightUnitId);
            ViewBag.InvoiceId = invoiceId;
            ViewBag.InvoiceName = (await _invoiceService.FindAsync(invoiceId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(instance);
        }

        // POST: /InvoiceLineItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ShrtDesc,Desc,InvoiceId,PriceBookId,ProductId,SalesPrice,Quantity,Weight,WeightUnitId,DiscountPercent,DiscountAmount,LineItemPrice,Serial,SpecialInstructions,Authorized,TimeStamp")] InvoiceLineItem instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                InvoiceLineItem model = await _service.FindAsync(instance.InvoiceId.ToString(), instance.PriceBookId.ToString(), instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  SalesAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("SalesAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Desc = instance.Authorized ? "[Authorized] " + instance.Desc : "[UnAuthorized] " + instance.Desc;
                    }
                }
                //If user is not in role of SalesAuthorize and record is Authorized, stop editng and show appropriate message.
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
                model.DiscountPercent = instance.DiscountPercent;
                model.SalesPrice = instance.SalesPrice;
                model.SpecialInstructions = instance.SpecialInstructions;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "ShrtDesc", "Desc", "InvoiceId", "PriceBookId", "ProductId", "SalesPrice", "Quantity", "Weight", "WeightUnitId", "DiscountPercent", "DiscountAmount", "LineItemPrice", "Serial", "SpecialInstructions", "Authorized" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { invoiceId = instance.InvoiceId.ToString(), priceBookId = instance.PriceBookId.ToString(), productId = instance.ProductId.ToString() });
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

        // GET: /InvoiceLineItem/Delete/5
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid invoiceId, Guid priceBookId, Guid productId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            InvoiceLineItem model = await _service.FindAsync(invoiceId.ToString(), priceBookId.ToString(), productId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: /InvoiceLineItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid invoiceId, Guid priceBookId, Guid productId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    InvoiceLineItem model = await _service.FindAsync(invoiceId.ToString(), priceBookId.ToString(), productId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: InvoiceId=" + invoiceId + " PriceBookId=" + priceBookId + " ProductId=" + productId);
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
