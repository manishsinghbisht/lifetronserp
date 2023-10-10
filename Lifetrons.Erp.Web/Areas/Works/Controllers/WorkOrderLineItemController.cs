using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using Lifetrons.Erp.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Works.Controllers
{
    [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit, WorksView")]
    public class WorkOrderLineItemController : BaseController
    {
        private readonly IOrderLineItemService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;

        private readonly IOrderService _orderService;
        private readonly IPriceBookLineItemService _priceBookLineItemService;
        private readonly IWeightUnitService _weightUnitService;
        
        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        // GET: /PriceBookLineItem/
        public WorkOrderLineItemController(IOrderLineItemService service, IUnitOfWorkAsync unitOfWork, IOrderService orderService,
            IPriceBookService priceBookService, IPriceBookLineItemService priceBookLineItemService, IWeightUnitService weightUnitService)
        {
            _service = service;
            _unitOfWork = unitOfWork;

            //Price Book Services
            _orderService = orderService;
            _priceBookLineItemService = priceBookLineItemService;
            _weightUnitService = weightUnitService;
        }

        // GET: /OrderLineItem/
        public ActionResult Index(string orderId)
        {
            ViewBag.OrderId = orderId;
            ViewBag.PriceBookId = null;

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = _service.SelectLineItems(orderId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (model.Any())
            {
                ViewBag.PriceBookId = model.FirstOrDefault().PriceBookId;
            }

            return PartialView(model);
        }

        // GET: /WorkOrderLineItem/Details/5
        public async Task<ActionResult> Details(string orderId, string priceBookId, string productId)
        {
            if (priceBookId == null || productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var instance = await _service.FindAsync(orderId, priceBookId, productId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.Order = await _orderService.FindAsync(instance.OrderId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            instance.AspNetUser = await AspNetUserService.FindAsync(instance.CreatedBy); //Created
            instance.AspNetUser1 = await AspNetUserService.FindAsync(instance.ModifiedBy); // Modified
            instance.WeightUnit = await _weightUnitService.FindAsync(instance.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.PriceBookLineItem = await _priceBookLineItemService.FindAsync(instance.PriceBookId.ToString(), instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.PriceBook = instance.PriceBookLineItem.PriceBook;

            return View(instance);
        }

        // GET: /OrderLineItem/Edit/5
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        public async Task<ActionResult> Edit(string orderId, string priceBookId, string productId)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(priceBookId) || string.IsNullOrEmpty(productId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            OrderLineItem instance = await _service.FindAsync(orderId, priceBookId, productId, applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "WorksAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("WorksAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.WeightUnitId);
            ViewBag.ProductionWeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ProductionWeightUnitId);

            ViewBag.OrderId = orderId;
            ViewBag.OrderName = (await _orderService.FindAsync(orderId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(instance);
        }

        // POST: /OrderLineItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ShrtDesc,Desc,OrderId,PriceBookId,ProductId,SalesPrice,Quantity,Weight,WeightUnitId,DiscountPercent,DiscountAmount,LineItemPrice,Serial,SpecialInstructions,ProductionQuantity,ProductionWeight,ProductionWeightUnitId,ProductionInstructions,Authorized,TimeStamp")] OrderLineItem instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                OrderLineItem model = await _service.FindAsync(instance.OrderId.ToString(), instance.PriceBookId.ToString(), instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  WorksAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("WorksAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Desc = instance.Authorized ? "[Authorized] " + instance.Desc : "[UnAuthorized] " + instance.Desc;
                    }
                }
                //If user is not in role of WorksAuthorize and record is Authorized, stop editng and show appropriate message.
                else if (model.Authorized)
                {
                    TempData["CustomErrorMessage"] = "Authorized record cannot be edited. Please Un-authorize the record to enable editing." +
                                                     "You should have authorization rights to authorize/unauthorize records.";
                    return RedirectToAction("Error", "Error", new { message = TempData["CustomErrorMessage"] });
                }

                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;

                model.ProductionQuantity = instance.ProductionQuantity;
                model.ProductionWeight = instance.ProductionWeight;
                model.ProductionWeightUnitId = instance.ProductionWeightUnitId;
                model.ProductionInstructions = instance.ProductionInstructions;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "ShrtDesc", "Desc", "OrderId", "PriceBookId", "ProductId", "SalesPrice", "Quantity", "Weight", "WeightUnitId", "DiscountPercent", "DiscountAmount", "LineItemPrice", "Serial", "SpecialInstructions", "JobNo", "JobStatus", "ProductionQuantity", "ProductionWeight", "ProductionWeightUnitId", "ProductionInstructions", "Authorized" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { orderId = instance.OrderId.ToString(), priceBookId = instance.PriceBookId.ToString(), productId = instance.ProductId.ToString() });
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
