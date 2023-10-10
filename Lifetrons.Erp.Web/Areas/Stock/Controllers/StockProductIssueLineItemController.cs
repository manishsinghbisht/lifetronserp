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
using Task = System.Threading.Tasks.Task;

namespace Lifetrons.Erp.Stock.Controllers
{
    [EsAuthorize(Roles = "StockAuthorize, StockEdit, StockView")]
    public class StockProductIssueLineItemController : BaseController
    {
        private readonly IStockProductIssueService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;

        private readonly IStockIssueHeadService _stockIssueHeadService;
        private readonly IWeightUnitService _weightUnitService;

        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        [Dependency]
        public IProductService ProductService { get; set; }

        // GET: /StockIssueLineItem/
        public StockProductIssueLineItemController(IStockProductIssueService service, IUnitOfWorkAsync unitOfWork, IStockIssueHeadService stockIssueHeadService,
            IWeightUnitService weightUnitService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _stockIssueHeadService = stockIssueHeadService;
            _weightUnitService = weightUnitService;
        }

        // GET: /StockIssueLineItem/
        public ActionResult Index(string stockIssueId)
        {
            ViewBag.StockIssueId = stockIssueId;

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = _service.SelectLineItems(stockIssueId, applicationUser.Id, applicationUser.OrgId.ToString());

            return PartialView(model);
        }

        // GET: /StockIssueLineItem/Details/5
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
            //instance.Item = await ItemService.FindAsync(instance.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Product = await ProductService.FindAsync(instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.StockIssueId = instance.StockIssueId;
            ViewBag.StockIssueHeadName = (await _stockIssueHeadService.FindAsync(instance.StockIssueId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(instance);
        }

        // GET: /StockIssueLineItem/Create
        [EsAuthorize(Roles = "StockAuthorize, StockEdit")]
        public async Task<ActionResult> Create(string stockIssueId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            //ViewBag.ItemId = new SelectList(await ItemService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ProductId = new SelectList(await ProductService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            ViewBag.StockIssueId = stockIssueId;
            ViewBag.StockIssueHeadName = (await _stockIssueHeadService.FindAsync(stockIssueId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            return View();
        }
        
        // POST: /StockIssueLineItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "StockAuthorize, StockEdit")]
        public async Task<ActionResult> Create([Bind(Include = "StockIssueId,Serial,ProductId,JobNo,CaseNo,TaskNo,Quantity,Weight,WeightUnitId,ExtraQuantity,DeliveryDate,Remark,CustomColumn1,Authorized")] StockProductIssue instance)
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
                instance.DeliveryDate = ControllerHelper.ConvertDateTimeToUtc(instance.DeliveryDate, User.TimeZone());

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
        [EsAuthorize(Roles = "StockAuthorize, StockEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            StockProductIssue instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return HttpNotFound();
            }

            await SetupInstanceForm(applicationUser, instance);

            return View(instance);
        }

        private async Task SetupInstanceForm(ApplicationUser applicationUser, StockProductIssue instance)
        {
            ViewBag.WeightUnitId =
                new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.WeightUnitId);
            ViewBag.ProductId =
                 new SelectList(await ProductService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                     "Name", instance.ProductId);

            ViewBag.StockIssueId = instance.StockIssueId;
            ViewBag.StockIssueHeadName =
                (await
                    _stockIssueHeadService.FindAsync(instance.StockIssueId.ToString(), applicationUser.Id,
                        applicationUser.OrgId.ToString())).Name;
        }

        // POST: /StockIssueLineItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "StockAuthorize, StockEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,StockIssueId,Serial,ProductId,JobNo,CaseNo,TaskNo,Quantity,Weight,WeightUnitId,ExtraQuantity,DeliveryDate,Remark,CustomColumn1,Authorized,Active,TimeStamp")] StockProductIssue instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                StockProductIssue model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  StockAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("StockAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Remark = instance.Authorized ? "[Authorized] " + instance.Remark : "[UnAuthorized] " + instance.Remark;
                    }
                }
                //If user is not in role of StockAuthorize and record is Authorized, stop editng and show appropriate message.
                else if (model.Authorized)
                {
                    TempData["CustomErrorMessage"] = "Authorized record cannot be edited. Please Un-authorize the record to enable editing." +
                                                     "You should have authorization rights to authorize/unauthorize records.";
                    return RedirectToAction("Error", "Error", new { message = TempData["CustomErrorMessage"] });
                }

                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;
                model.StockIssueId = instance.StockIssueId;

                model.Serial = instance.Serial;
                //model.ItemId = instance.ItemId.ToGuid();
                model.ProductId = instance.ProductId.ToGuid();
                model.JobNo = instance.JobNo;
                model.CaseNo = instance.CaseNo;
                model.TaskNo = instance.TaskNo;
                model.Quantity = instance.Quantity;
                model.Weight = instance.Weight;
                model.WeightUnitId = instance.WeightUnitId;
                model.ExtraQuantity = instance.ExtraQuantity;
                model.DeliveryDate = ControllerHelper.ConvertDateTimeToUtc(instance.DeliveryDate, User.TimeZone());
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
                        var membersToCompare = new List<string>() { "Serial", "ProductId", "JobNo", "CaseNo", "TaskNo", "Quantity", "Weight", "WeightUnitId", "ExtraQuantity", "DeliveryDate", "Authorized", "Active" };
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
        [EsAuthorize(Roles = "StockAuthorize")]
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
            //model.Item = await ItemService.FindAsync(model.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Product = await ProductService.FindAsync(model.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());           
            model.StockIssueHead = await _stockIssueHeadService.FindAsync(model.StockIssueId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            
            ViewBag.StockIssueId = model.StockIssueId;
            ViewBag.StockIssueHeadName = model.StockIssueHead.Name;

           
            return View(model);
        }

        // POST: /StockIssueLineItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "StockAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                string parentGuid = string.Empty;
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    StockProductIssue model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    parentGuid = model.StockIssueId.ToString();
                    string logEntry = "Deleted StockIssueDetail: Id=" + model.Id + " StockIssueId=" + model.StockIssueId + " ProductId=" + model.ProductId + "JobNo=" + model.JobNo + "CaseNo=" + model.CaseNo + "TaskNo=" + model.TaskNo;
                    _service.Delete(id.ToString()); 

                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, logEntry);
                }
                return RedirectToAction("Details", "StockIssue", new { id = parentGuid });

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
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(dbUpdateException));
            string innerExceptionMessage = dbUpdateException.InnerException.InnerException.Message;
            TempData["CustomErrorMessage"] = innerExceptionMessage;
            TempData["CustomErrorDetail"] = dbUpdateException.InnerException.Message;

            if (innerExceptionMessage.Contains("UQ") ||
                innerExceptionMessage.Contains("Primary Key") ||
                innerExceptionMessage.Contains("PK"))
            {
                if (innerExceptionMessage.Contains("PriceBookId_ProductId"))
                {
                    TempData["CustomErrorMessage"] = "Duplicate Item. Item already exists.";
                    AddErrors("Duplicate Item. Item already exists.");
                }
                else if (innerExceptionMessage.Contains("UQ_StockIssueDetail_StockIssueId_ProductId"))
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
            else if (innerExceptionMessage.Contains("CaseNo"))
            {
                AddErrors("Invalid Job No.");
                TempData["CustomErrorMessage"] = "Invalid Case No.";
            }
            else if (innerExceptionMessage.Contains("TaskNo"))
            {
                AddErrors("Invalid Job No.");
                TempData["CustomErrorMessage"] = "Invalid Task No.";
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
