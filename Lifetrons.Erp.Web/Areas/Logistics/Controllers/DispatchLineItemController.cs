using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.WebPages;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using Lifetrons.Erp.Web.Models;

namespace Lifetrons.Erp.Logistics.Controllers
{
    [EsAuthorize(Roles = "LogisticsAuthorize, LogisticsEdit, LogisticsView")]
    public class DispatchLineItemController : BaseController
    {
        private readonly IDispatchLineItemService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IWeightUnitService _weightUnitService;

        [Dependency]
        public IDispatchService DispatchService { get; set; }

        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        [Dependency]
        public IOrderService OrderService { get; set; }

        [Dependency]
        public IOrderLineItemService OrderLineItemService { get; set; }


        // GET: /DispatchLineItem/
        public DispatchLineItemController(IDispatchLineItemService service, IUnitOfWorkAsync unitOfWork, IWeightUnitService weightUnitService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _weightUnitService = weightUnitService;
        }

        // GET: /DispatchLineItem/
        public ActionResult Index(string dispatchId)
        {
            ViewBag.DispatchId = dispatchId;

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = _service.SelectLineItems(dispatchId, applicationUser.Id, applicationUser.OrgId.ToString());

            return PartialView(model);
        }

        // GET: /DispatchLineItem/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = await AspNetUserService.FindAsync(instance.CreatedBy); //Created
            instance.AspNetUser1 = await AspNetUserService.FindAsync(instance.ModifiedBy); // Modified
            instance.WeightUnit = await _weightUnitService.FindAsync(instance.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Order = await OrderService.FindAsync(instance.OrderId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.OrderLineItem = await OrderLineItemService.FindAsync(instance.OrderId.ToString(), instance.PriceBookId.ToString(), instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.DispatchId = instance.DispatchId;
            ViewBag.DispatchName = (await DispatchService.FindAsync(instance.DispatchId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(instance);
        }

        // GET: /DispatchLineItem/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> QRResponse(string id)
        {
            var instance = await _service.QRFindAsync(id);
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = await AspNetUserService.FindAsync(instance.CreatedBy); //Created
            instance.AspNetUser1 = await AspNetUserService.FindAsync(instance.ModifiedBy); // Modified
            instance.WeightUnit = await _weightUnitService.FindAsync(instance.WeightUnitId.ToString(), instance.CreatedBy, instance.OrgId.ToString());
            instance.Order = await OrderService.FindAsync(instance.OrderId.ToString(), instance.CreatedBy, instance.OrgId.ToString());
            instance.OrderLineItem = await OrderLineItemService.FindAsync(instance.OrderId.ToString(), instance.PriceBookId.ToString(), instance.ProductId.ToString(), instance.CreatedBy, instance.OrgId.ToString());

            ViewBag.DispatchId = instance.DispatchId;
            ViewBag.DispatchName = (await DispatchService.FindAsync(instance.DispatchId.ToString(), instance.CreatedBy, instance.OrgId.ToString())).Name;

            return View(instance);
        }

        // GET: /DispatchLineItem/Create
        [EsAuthorize(Roles = "LogisticsAuthorize, LogisticsEdit")]
        public async Task<ActionResult> Create(string dispatchId)
        {
            await InitializeCreateForm(dispatchId);

            return View();
        }

        private async System.Threading.Tasks.Task InitializeCreateForm(string dispatchId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var dispatch = await DispatchService.FindAsync(dispatchId, applicationUser.Id, applicationUser.OrgId.ToString());
            if (dispatch.OrderId != null)
            {
                //Single Order Dispatch
                ViewBag.IsMultiOrderDispatch = false;
                ViewBag.OrderId = dispatch.OrderId;
                ViewBag.OrderLineItemId = new SelectList(await OrderLineItemService.SelectAsyncLineItems(dispatch.OrderId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Product.Name");
            }
            else
            {
                //Combine Multiple Orders in dispatch
                ViewBag.IsMultiOrderDispatch = true;
                ViewBag.OrderId = new SelectList(await OrderService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
                ViewBag.OrderLineItemId = new SelectList(await OrderLineItemService.SelectAsyncLineItems(Guid.NewGuid().ToString(), applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Product.Name");
            }

            ViewBag.DispatchId = dispatchId;
            ViewBag.DispatchName = dispatch.Name;
            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
        }

        // POST: /DispatchLineItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "LogisticsAuthorize, LogisticsEdit")]
        public async Task<ActionResult> Create([Bind(Include = "DispatchId,OrderId,OrderLineItemId,PriceBookId,ProductId,Serial,ShrtDesc,Quantity,Weight,WeightUnitId,Remark,Authorized")] DispatchLineItem instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                ModelState.Clear();

                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

                instance.Id = Guid.NewGuid();
                instance.OrgId = applicationUser.OrgId.ToSysGuid();
                instance.CreatedBy = applicationUser.Id;
                instance.CreatedDate = DateTime.UtcNow;
                instance.ModifiedBy = applicationUser.Id;
                instance.ModifiedDate = DateTime.UtcNow;
                instance.Active = true;

                var orderLineItem = await OrderLineItemService.SelectSingleAsync(instance.OrderLineItemId.ToString(), instance.OrderId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                instance.ProductId = orderLineItem.ProductId;
                instance.PriceBookId = orderLineItem.PriceBookId;

                //Check for duplicate entry and unique records
                var existingRecords = await _service.SelectAsyncLineItems(instance.DispatchId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                foreach (var record in existingRecords)
                {
                    if (record.OrderLineItemId == instance.OrderLineItemId)
                    {
                        AddErrors("Duplicate entry! Record already exists.");
                        TempData["CustomErrorMessage"] = "Duplicate entry! Record already exists.";
                    }
                }

                try
                {
                    if (TryValidateModel(instance))
                    {
                        _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = instance.Id.ToString() });
                    }
                    else
                    {
                        var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                                            .Select(x => new { x.Key, x.Value.Errors })
                                            .ToArray();
                        foreach (var item in errors)
                        {
                            AddErrors(item.Errors.FirstOrDefault().ErrorMessage);
                        }
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

                await InitializeCreateForm(instance.DispatchId.ToString());
                return View(instance);
            }

            return HttpNotFound();
        }

        // GET: /DispatchLineItem/Edit/5
        [EsAuthorize(Roles = "LogisticsAuthorize, LogisticsEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            DispatchLineItem instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return HttpNotFound();
            }

            var orderLineItem = await OrderLineItemService.SelectSingleAsync(instance.OrderLineItemId.ToString(), instance.OrderId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            instance.Product = orderLineItem.Product;
            instance.PriceBook = orderLineItem.PriceBook;

            ViewBag.DispatchId = id;
            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.WeightUnitId);

            return View(instance);
        }

        // POST: /DispatchLineItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "LogisticsAuthorize, LogisticsEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,DispatchId,Serial,ShrtDesc,Quantity,Weight,WeightUnitId,Remark,Authorized,Active,TimeStamp")] DispatchLineItem instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                DispatchLineItem model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  LogisticsAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("LogisticsAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Remark = instance.Authorized ? "[Authorized] " + instance.Remark : "[UnAuthorized] " + instance.Remark;
                    }
                }
                //If user is not in role of LogisticsAuthorize and record is Authorized, stop editng and show appropriate message.
                else if (model.Authorized)
                {
                    TempData["CustomErrorMessage"] = "Authorized record cannot be edited. Please Un-authorize the record to enable editing." +
                                                     "You should have authorization rights to authorize/unauthorize records.";
                    return RedirectToAction("Error", "Error", new { message = TempData["CustomErrorMessage"] });
                }

                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;
                model.DispatchId = instance.DispatchId;
                string instanceRemark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                model.Remark = model.Remark.IsEmpty() ? instanceRemark : model.Remark + "\n" + instanceRemark;

                model.ShrtDesc = instance.ShrtDesc;
                model.Serial = instance.Serial;
                model.Quantity = instance.Quantity;
                model.Weight = instance.Weight;
                model.WeightUnitId = instance.WeightUnitId;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Id", "DispatchId", "OrderId", "OrderLineItemId", "PriceBookId", "ProductId", "Serial", "ShrtDesc", "Quantity", "Weight", "WeightUnitId", "Remark", "Authorized" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { id = instance.Id.ToString() });
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

        // GET: /DispatchLineItem/Delete/5
        [EsAuthorize(Roles = "LogisticsAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid id)
        {
            if (id.ToString().IsNullOrWhiteSpace()) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            if (model == null) return HttpNotFound();

            model.AspNetUser = await AspNetUserService.FindAsync(model.CreatedBy); //Created
            model.AspNetUser1 = await AspNetUserService.FindAsync(model.ModifiedBy); // Modified
            model.WeightUnit = await _weightUnitService.FindAsync(model.WeightUnitId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Order = await OrderService.FindAsync(model.OrderId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.OrderLineItem = await OrderLineItemService.FindAsync(model.OrderId.ToString(), model.PriceBookId.ToString(), model.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.DispatchId = model.DispatchId;
            ViewBag.DispatchIdName = (await DispatchService.FindAsync(model.DispatchId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(model);
        }

        // POST: /DispatchLineItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "LogisticsAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                string parentGuid = string.Empty;
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    DispatchLineItem model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    parentGuid = model.DispatchId.ToString();
                    string logEntry = "Deleted DispatchLineItem: Id=" + model.Id + " DispatchId=" + model.DispatchId + " OrderId=" + model.OrderId + " OrderLineItemId=" + model.OrderLineItemId + " ProductId=" + model.ProductId + " PriceBookId=" + model.PriceBookId + " Quantity=" + model.Quantity;
                    _service.Delete(id.ToString());

                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, logEntry);
                }
                return RedirectToAction("Details", "Dispatch", new { id = parentGuid });

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

        /// <summary>
        /// This method is called to update dropdown through ajax request
        /// </summary>
        /// <param name="stringifiedParam">Serialized parameter to receive data from client side</param>
        /// <returns>Java script serialized string array to be used with JSON parsing</returns>
        public async virtual Task<JsonResult> ProcessJsonResponseFillOrderLineItemsDropDown(string stringifiedParam)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //var param = JsonConvert.DeserializeObject(stringifiedParam); //No need of deserialization here as only single value is passed from client
            var param = stringifiedParam;

            var response = new JsonResult();
            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var select = new SelectList(await OrderLineItemService.SelectAsyncLineItems(param, applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Product.Name");

            // Convert anonymous object to JSON response
            response = Json(select, JsonRequestBehavior.AllowGet);

            // Return JSON
            return response;
        }

        public DispatchStatus DispatchStatus(string orderLineItemId)
        {
            var dispatchedLineItems = _service.DispatchStatus(orderLineItemId);
            DispatchStatus dispatchStatus = new Web.Models.DispatchStatus();
            foreach (var item in dispatchedLineItems)
            {
                dispatchStatus.OrderQuantity = item.OrderLineItem.Quantity;
                dispatchStatus.AlreadyDispatchedQuantity += item.Quantity;
                dispatchStatus.AlreadyDispatchedWeight += item.Weight ?? 0;
            }

            return dispatchStatus;
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
