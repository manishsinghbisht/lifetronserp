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

namespace Lifetrons.Erp.Sales.Controllers
{
    [EsAuthorize(Roles = "SalesAuthorize, SalesEdit, SalesView")]
    public class OrderLineItemController : BaseController
    {
        private readonly IOrderLineItemService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;

        private readonly IOrderService _orderService;
        private readonly IPriceBookLineItemService _priceBookLineItemService;
        private readonly IWeightUnitService _weightUnitService;
        
        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        // GET: /PriceBookLineItem/
        public OrderLineItemController(IOrderLineItemService service, IUnitOfWorkAsync unitOfWork, IOrderService orderService,
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

        // GET: /OrderLineItem/Details/5
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

        // GET: /Order/Create
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create(string orderId, string priceBookId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.OrderId = orderId;
            ViewBag.OrderName = (await _orderService.FindAsync(orderId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.PriceBookId = null;

            if (!string.IsNullOrEmpty(priceBookId))
            {
                ViewBag.PriceBookId = priceBookId;
            }

            var model = InitializeCreateModel(priceBookId, applicationUser);

            return View(model);
        }

        private OrderLineItem InitializeCreateModel(string priceBookId, ApplicationUser applicationUser)
        {
            //Load product from PricBookLineItemSelector
            var model = new OrderLineItem();

            if (TempData["PriceBookLineItemSelectorSelectedId"] != null)
            {
                try
                {
                    var selectedProductId = TempData["PriceBookLineItemSelectorSelectedId"].ToString().ToSysGuid();
                    PriceBookLineItem priceBookLineItem = _priceBookLineItemService.Find(priceBookId, selectedProductId.ToString(),
                                applicationUser.Id, applicationUser.OrgId.ToString());
                    model = new OrderLineItem()
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

        // POST: /OrderLineItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create([Bind(Include = "ShrtDesc,Desc,OrderId,PriceBookId,ProductId,SalesPrice,Quantity,Weight,WeightUnitId,DiscountPercent,DiscountAmount,LineItemPrice,Serial,SpecialInstructions,Authorized")] OrderLineItem instance)
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

            return RedirectToAction("Details", new { orderId = instance.OrderId.ToString(), priceBookId = instance.PriceBookId.ToString(), productId = instance.ProductId.ToString() });
        }

        // GET: /OrderLineItem/Edit/5
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
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

            //If instance.Authorized, only user with "SalesAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("SalesAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.WeightUnitId);

            ViewBag.OrderId = orderId;
            ViewBag.OrderName = (await _orderService.FindAsync(orderId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(instance);
        }

        // POST: /OrderLineItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ShrtDesc,Desc,OrderId,PriceBookId,ProductId,SalesPrice,Quantity,Weight,WeightUnitId,DiscountPercent,DiscountAmount,LineItemPrice,Serial,SpecialInstructions,ProductionQuantity,ProductionWeight,ProductionWeightUnitId,ProductionInstructions,Authorized,TimeStamp")] OrderLineItem instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                OrderLineItem model = await _service.FindAsync(instance.OrderId.ToString(), instance.PriceBookId.ToString(), instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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

        // GET: /OrderLineItem/Delete/5
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid orderId, Guid priceBookId, Guid productId)
        {
            if (orderId.ToString().IsNullOrWhiteSpace() || priceBookId.ToString().IsNullOrWhiteSpace() || productId.ToString().IsNullOrWhiteSpace())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var model = await _service.FindAsync(orderId.ToString(), priceBookId.ToString(), productId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            model.Order = await _orderService.FindAsync(model.OrderId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            model.AspNetUser = Lifetrons.Erp.Helpers.ControllerHelper.GetAspNetUser(model.CreatedBy);
            model.AspNetUser1 = Lifetrons.Erp.Helpers.ControllerHelper.GetAspNetUser(model.ModifiedBy);
            model.WeightUnit = await _weightUnitService.FindAsync(model.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.PriceBookLineItem = await _priceBookLineItemService.FindAsync(model.PriceBookId.ToString(), model.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.PriceBook = model.PriceBookLineItem.PriceBook;

            return View(model);
        }

        // POST: /OrderLineItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid orderId, Guid priceBookId, Guid productId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    OrderLineItem model = await _service.FindAsync(orderId.ToString(), priceBookId.ToString(), productId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    
                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: OrderId=" + orderId + " PriceBookId=" + priceBookId + " ProductId=" + productId);
                }
                return RedirectToAction("Details", "Order", new { id = orderId });
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
        private void HandleDbUpdateException(DbUpdateException dbUpdateException)
        {
            string innerExceptionMessage = dbUpdateException.InnerException.InnerException.Message;
            TempData["CustomErrorMessage"] = innerExceptionMessage;
            TempData["CustomErrorDetail"] = dbUpdateException.InnerException.Message;

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
            else
            {
                AddErrors(dbUpdateException.Message + " - " + DateTime.UtcNow);
                TempData["CustomErrorMessage"] = dbUpdateException.Message + " - " + dbUpdateException.InnerException.Message;
            }
        }

        #endregion Handle Exception
    }
}
