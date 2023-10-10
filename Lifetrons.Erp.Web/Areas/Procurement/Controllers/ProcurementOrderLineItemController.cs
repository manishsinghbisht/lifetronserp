using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.Odbc;
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
using Task = System.Threading.Tasks.Task;

namespace Lifetrons.Erp.Procurement.Controllers
{
    [EsAuthorize(Roles = "ProcurementAuthorize, ProcurementEdit, ProcurementView")]
    public class ProcurementOrderLineItemController : BaseController
    {
        private readonly IProcurementOrderDetailService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;

        private readonly IProcurementOrderService _procurementOrderService;
        private readonly IWeightUnitService _weightUnitService;

        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        [Dependency]
        public IItemService ItemService { get; set; }

        [Dependency]
        public IProcurementRequestDetailService ProcurementRequestDetailService { get; set; }
        [Dependency]
        public IProcurementRequestService ProcurementRequestService { get; set; }

        // GET: /ProcurementOrderLineItem/
        public ProcurementOrderLineItemController(IProcurementOrderDetailService service, IUnitOfWorkAsync unitOfWork, IProcurementOrderService procurementOrderService,
            IWeightUnitService weightUnitService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _procurementOrderService = procurementOrderService;
            _weightUnitService = weightUnitService;
        }

        // GET: /ProcurementOrderLineItem/
        public ActionResult Index(string stockIssueId)
        {
            ViewBag.StockIssueId = stockIssueId;

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = _service.SelectLineItems(stockIssueId, applicationUser.Id, applicationUser.OrgId.ToString());

            return PartialView(model);
        }

        // GET: /ProcurementOrderLineItem/Details/5
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
            instance.ProcurementRequestDetail = await ProcurementRequestDetailService.FindAsync(instance.ProcurementRequestDetailId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Item = await ItemService.FindAsync(instance.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.ProcurementOrderId = instance.ProcurementOrderId;
            ViewBag.ProcurementOrderName = (await _procurementOrderService.FindAsync(instance.ProcurementOrderId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(instance);
        }

        // GET: /StockIssueLineItem/Create
        [EsAuthorize(Roles = "ProcurementAuthorize, ProcurementEdit")]
        public async Task<ActionResult> Create(string procurementOrderId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            
            //ViewBag.ItemId = new SelectList(await ItemService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            
           var requestLineItem = await ProcurementRequestDetailService.SelectAsyncPendingLineItems(applicationUser.Id, applicationUser.OrgId.ToString());
           
           var list = new List<SelectListItem>();
            requestLineItem.ForEach(p =>
            {
                var item = ItemService.Find(p.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                var selectListItem = new SelectListItem { Value = p.Id.ToString(), Text = item.Code + " - " + ControllerHelper.ConvertDateTimeToUtc(p.RequiredByDate, User.TimeZone())}; 
                list.Add(selectListItem);
            });

            var select = new SelectList(list, "Value", "Text");

            ViewBag.ProcurementRequestDetailId = select;
            
            ViewBag.ProcurementOrderId = procurementOrderId;
            ViewBag.ProcurementOrderName = (await _procurementOrderService.FindAsync(procurementOrderId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View();
        }
        
        // POST: /StockIssueLineItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ProcurementAuthorize, ProcurementEdit")]
        public async Task<ActionResult> Create([Bind(Include = "ProcurementOrderId,ProcurementRequestDetailId,ShrtDesc,Serial,ItemId,Quantity,Weight,WeightUnitId,SalesPrice,Status,DiscountPercent,Remark,Authorized,Active")] ProcurementOrderDetail instance)
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
                    return RedirectToAction("Details", new { id = instance.Id });
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEntityValidationException)
                {
                    HandleDbEntityValidationException(dbEntityValidationException);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbUpdateException)
                {
                    HandleDbUpdateException(dbUpdateException);
                }
                catch (Exception exception)
                {
                    HandleException(exception);
                }

                // If we got this far, something failed, redisplay form
                await SetupInstanceForm(applicationUser, instance);
                return View(instance);
            }

            return HttpNotFound();
        }

        // GET: /StockIssueLineItem/Edit/5
        [EsAuthorize(Roles = "ProcurementAuthorize, ProcurementEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ProcurementOrderDetail instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return HttpNotFound();
            }

            await SetupInstanceForm(applicationUser, instance);

            return View(instance);
        }

        private async Task SetupInstanceForm(ApplicationUser applicationUser, ProcurementOrderDetail instance)
        {
            ViewBag.WeightUnitId =
                new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.WeightUnitId);

            ViewBag.ProcurementRequestDetailId = new SelectList(await ProcurementRequestDetailService.SelectAsyncPendingLineItems(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "item.Code", instance.ProcurementRequestDetailId);

            ViewBag.ItemId = new SelectList(
                await ItemService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name",
                instance.ItemId);

            ViewBag.ProcurementOrderId = instance.ProcurementOrderId;
            ViewBag.ProcurementOrderName =
                (await
                    _procurementOrderService.FindAsync(instance.ProcurementOrderId.ToString(), applicationUser.Id,
                        applicationUser.OrgId.ToString())).Name;
        }

        // POST: /StockIssueLineItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ProcurementAuthorize, ProcurementEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProcurementOrderId,ProcurementRequestDetailId,ShrtDesc,Serial,ItemId,Quantity,Weight,WeightUnitId,SalesPrice,Status,DiscountPercent,Remark,Authorized,Active,TimeStamp")] ProcurementOrderDetail instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                ProcurementOrderDetail model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  ProcurementAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("ProcurementAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Remark = instance.Authorized ? "[Authorized] " + instance.Remark : "[UnAuthorized] " + instance.Remark;
                    }
                }
                //If user is not in role of ProcurementAuthorize and record is Authorized, stop editng and show appropriate message.
                else if (model.Authorized)
                {
                    TempData["CustomErrorMessage"] = "Authorized record cannot be edited. Please Un-authorize the record to enable editing." +
                                                     "You should have authorization rights to authorize/unauthorize records.";
                    return RedirectToAction("Error", "Error", new { message = TempData["CustomErrorMessage"] });
                }

                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;

                model.ShrtDesc = instance.ShrtDesc;
                model.ProcurementOrderId = instance.ProcurementOrderId;

                model.Serial = instance.Serial;
                model.ItemId = instance.ItemId.ToGuid();
                model.Quantity = instance.Quantity;
                model.Weight = instance.Weight;
                model.WeightUnitId = instance.WeightUnitId;
                model.SalesPrice = instance.SalesPrice;
                model.DiscountPercent = instance.DiscountPercent;
                model.Status = instance.Status;

                string instanceRemark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                model.Remark = model.Remark.IsEmpty() ? instanceRemark : model.Remark + "\n" + instanceRemark;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Id", "Serial", "ItemId", "Quantity", "Weight", "WeightUnitId", "ExtraQuantity", "RequiredByDate", "EstimatedCost", "Remark", "Authorized", "Active" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { id = instance.Id.ToString() });
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEntityValidationException)
                {
                    HandleDbEntityValidationException(dbEntityValidationException);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbUpdateException)
                {
                    HandleDbUpdateException(dbUpdateException);
                }
                catch (Exception exception)
                {
                    HandleException(exception);
                }

                // If we got this far, something failed, redisplay form
                await SetupInstanceForm(applicationUser, instance);
                return View(instance);
            }

            return HttpNotFound();
        }

        // GET: /StockIssueLineItem/Delete/5
        [EsAuthorize(Roles = "ProcurementAuthorize")]
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
            model.Item = await ItemService.FindAsync(model.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.ProcurementOrder = await _procurementOrderService.FindAsync(model.ProcurementOrderId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.ProcurementOrderId = model.ProcurementOrderId;
            ViewBag.ProcurementOrderName = model.ProcurementOrder.Name;
            
            return View(model);
        }

        // POST: /StockIssueLineItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ProcurementAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                string parentGuid = string.Empty;
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    ProcurementOrderDetail model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    parentGuid = model.ProcurementOrderId.ToString();
                    string logEntry = "Deleted ProcurementOrderDetail: Id=" + model.Id + " ProcurementOrderId=" + model.ProcurementOrderId + " ItemId=" + model.ItemId;
                    _service.Delete(id.ToString()); 

                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, logEntry);
                }
                return RedirectToAction("Details", "ProcurementRequest", new { id = parentGuid });

            }

            return HttpNotFound();
        }

        /// <summary>
        /// This method is called to update address through ajax request
        /// </summary>
        /// <param name="stringifiedParam">Serialized parameter to receive data from client side</param>
        /// <returns>Java script serialized string array to be used with JSON parsing</returns>
        public async virtual Task<JsonResult> PostBackProcurementRequest(string stringifiedParam)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //var param = JsonConvert.DeserializeObject(stringifiedParam); //No need of deserialization here as only single value is passed from client
            var param = stringifiedParam;

            var response = new JsonResult();
            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var procurementRequest = await ProcurementRequestDetailService.FindAsync(param, applicationUser.Id, applicationUser.OrgId.ToString());
            var item = await ItemService.FindAsync(procurementRequest.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            //var selectListItem = new SelectListItem { Value = item.Id.ToString(), Text = item.Code };
            //var list = new List<SelectListItem>();
            //list.Add(selectListItem);
            //var select = new SelectList(list, "Value", "Text");
            //// Convert anonymous object to JSON response
            //response = Json(select, JsonRequestBehavior.AllowGet);

            var returnData = new
            {
                value = item.Id,
                text = item.Code
            };

            // Convert anonymous object to JSON response
            response = Json(returnData, JsonRequestBehavior.AllowGet);

            // Return JSON
            return response;
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
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(dbUpdateException));
            string innerExceptionMessage = dbUpdateException.InnerException.InnerException.Message;
            TempData["CustomErrorMessage"] = innerExceptionMessage;
            TempData["CustomErrorDetail"] = dbUpdateException.InnerException.Message;

            if (innerExceptionMessage.Contains("UQ") ||
                innerExceptionMessage.Contains("Primary Key") ||
                innerExceptionMessage.Contains("PK"))
            {
                if (innerExceptionMessage.Contains("PriceBookId_ItemId"))
                {
                    TempData["CustomErrorMessage"] = "Duplicate Item. Item already exists.";
                    AddErrors("Duplicate Item. Item already exists.");
                }
                else if (innerExceptionMessage.Contains("UQ_ProcurementOrderDetail_StockIssueId_ItemId_ProductId"))
                {
                    TempData["CustomErrorMessage"] = "Duplicate Item or Product. Key record already exists.";
                    AddErrors("Duplicate Item or Product. Key record already exists.");
                }
                else
                {
                    TempData["CustomErrorMessage"] = "Duplicate Name or Code. Key record already exists.";
                    AddErrors("Duplicate Name or Code. Key record already exists.");
                }
            }
            else if (innerExceptionMessage.Contains("_CheckQuantity"))
            {
                TempData["CustomErrorMessage"] = "Invalid quantity.";
                AddErrors("Invalid quantity.");
            }
            else if (innerExceptionMessage.Contains("JobNo"))
            {
                AddErrors("Invalid Job No.");
                TempData["CustomErrorMessage"] = "Invalid Job No.";

            }
            else
            {
                AddErrors(dbUpdateException.Message.ToString() + " - " + DateTime.UtcNow);
                TempData["CustomErrorMessage"] = dbUpdateException.Message.ToString() + " - " + dbUpdateException.InnerException.Message.ToString();

            }
        }

        private string HandleDbEntityValidationException(DbEntityValidationException dbEntityValidationException)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(dbEntityValidationException));
            // Retrieve the error messages as a list of strings.
            var errorMessages = dbEntityValidationException.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

            // Join the list to a single string.
            var fullErrorMessage = string.Join("; ", errorMessages);

            // Combine the original exception message with the new one.
            var exceptionMessage = string.Concat(dbEntityValidationException.Message, " The validation errors are: ", fullErrorMessage);

            // Throw a new DbEntityValidationException with the improved exception message.
            TempData["CustomErrorMessage"] = exceptionMessage;
            TempData["CustomErrorDetail"] = dbEntityValidationException.InnerException.Message;
            AddErrors(exceptionMessage + " ___" + DateTime.UtcNow);

            return exceptionMessage;
        }

        private void HandleException(Exception exception)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
            TempData["CustomErrorMessage"] = exception.Message;
            TempData["CustomErrorDetail"] = exception.InnerException.Message;
            AddErrors(exception.Message.ToString() + " ___" + DateTime.UtcNow);
        }

        private void AddErrors(string errorMessage)
        {
            ModelState.AddModelError("", errorMessage);
        }

        #endregion Handle Exception
    }
}
