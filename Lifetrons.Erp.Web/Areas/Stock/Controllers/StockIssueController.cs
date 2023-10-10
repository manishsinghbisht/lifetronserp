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
using PagedList;
using Repository;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using WebGrease.Css.Extensions;
using Task = System.Threading.Tasks.Task;

namespace Lifetrons.Erp.Stock.Controllers
{
    [EsAuthorize(Roles = "StockAuthorize, StockEdit, StockView")]
    public class StockIssueController : BaseController
    {
        [Dependency]
        public IEmployeeService EmployeeService { get; set; }

        [Dependency]
        public IEnterpriseStageService EnterpriseStageService { get; set; }

        [Dependency]
        public IProcessService ProcessService { get; set; }

        [Dependency]
        public IStockItemIssueService StockItemIssueService { get; set; }

        [Dependency]
        public IStockProductIssueService StockProductIssueService { get; set; }

        private readonly IStockIssueHeadService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        public StockIssueController(IStockIssueHeadService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
        }

        // GET: /StockIssue/
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.ModifiedDate).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: /StockIssue/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            StockIssueHead instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified

            instance.Employee = await EmployeeService.FindAsync(instance.EmployeeId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.EnterpriseStage = await EnterpriseStageService.FindAsync(instance.IssuedByEnterpriseStageId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Process = await ProcessService.FindAsync(instance.IssuedByProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.EnterpriseStage1 = await EnterpriseStageService.FindAsync(instance.IssuedToEnterpriseStageId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Process1 = await ProcessService.FindAsync(instance.IssuedToProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            //Always be Stock for stock module
            ViewBag.IssuedByEnterpriseStageId = instance.IssuedByEnterpriseStageId;
            ViewBag.IssuedByEnterpriseStageName = (await EnterpriseStageService.FindAsync(instance.IssuedByEnterpriseStageId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.IssuedByProcessId = instance.IssuedByProcessId;
            ViewBag.IssuedByProcessName = (await ProcessService.FindAsync(instance.IssuedByProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            instance.StockItemIssues = (await StockItemIssueService.SelectAsyncLineItems(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).ToList();
            instance.StockProductIssues = (await StockProductIssueService.SelectAsyncLineItems(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).ToList();
            
            //Set id to pass on detail partial page
            ViewBag.StockIssueId = instance.Id;
            ViewBag.Name = instance.Name;

            return View(instance);
        }

        // GET: /StockIssue/Create
        [EsAuthorize(Roles = "StockAuthorize, StockEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.EmployeeId = new SelectList(await EmployeeService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.IssuedToEnterpriseStageId = new SelectList(await EnterpriseStageService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.IssuedToProcessId = new SelectList(await ProcessService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            //Always be Stock for stock module
            ViewBag.IssuedByProcessId = new SelectList(await ProcessService.SelectEnterpriseStageProcessAsync(Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"], applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.IssuedByEnterpriseStageId = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"];
            ViewBag.IssuedByEnterpriseStageName = (await EnterpriseStageService.FindAsync(Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"], applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View();
        }

        // POST: /StockIssue/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "StockAuthorize, StockEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Name,RefNo,JobType,Date,IssuedByEnterpriseStageId,IssuedByProcessId,IssuedToEnterpriseStageId,IssuedToProcessId,EmployeeId,Remark,Authorized")] StockIssueHead instance)
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
                instance.Name = instance.Name + Helper.SysSeparator + DateTime.UtcNow;

                instance.IssuedByProcessId = instance.IssuedByProcessId.ToGuid();
                instance.IssuedByEnterpriseStageId = (await ProcessService.FindAsync(instance.IssuedByProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).EnterpriseStageId;

                instance.IssuedToProcessId = instance.IssuedToProcessId.ToGuid();
                instance.IssuedToEnterpriseStageId = (await ProcessService.FindAsync(instance.IssuedToProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).EnterpriseStageId;

                instance.EmployeeId = instance.EmployeeId.ToGuid();
                instance.Date = ControllerHelper.ConvertDateTimeToUtc(instance.Date, User.TimeZone());

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

        // GET: /StockIssue/Edit/5
        [EsAuthorize(Roles = "StockAuthorize, StockEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            StockIssueHead instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "StockAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("StockAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            await SetupInstanceForm(applicationUser, instance);

            return View(instance);
        }

        private async Task SetupInstanceForm(ApplicationUser applicationUser, StockIssueHead instance)
        {
            ViewBag.EmployeeId =
                new SelectList(await EmployeeService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.EmployeeId);
            ViewBag.IssuedToEnterpriseStageId =
                new SelectList(await EnterpriseStageService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()),
                    "Id", "Name", instance.IssuedToEnterpriseStageId);
            ViewBag.IssuedToProcessId =
                new SelectList(await ProcessService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.IssuedToProcessId);

            //Always be Stock for stock module
            ViewBag.IssuedByProcessId =
                new SelectList(
                    await
                        ProcessService.SelectEnterpriseStageProcessAsync(
                            Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"], applicationUser.Id,
                            applicationUser.OrgId.ToString()), "Id", "Name", instance.IssuedByProcessId);
            ViewBag.IssuedByEnterpriseStageId = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"];
            ViewBag.IssuedByEnterpriseStageName =
                (await
                    EnterpriseStageService.FindAsync(Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"],
                        applicationUser.Id, applicationUser.OrgId.ToString())).Name;
        }

        // POST: /StockIssue/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "StockAuthorize, StockEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,RefNo,JobType,Date,IssuedByEnterpriseStageId,IssuedByProcessId,IssuedToEnterpriseStageId,IssuedToProcessId,EmployeeId,Remark,Authorized,TimeStamp")] StockIssueHead instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                StockIssueHead model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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
                model.Name = instance.Name;
                string instanceRemark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                model.Remark = model.Remark.IsEmpty() ? instanceRemark : model.Remark + "\n" + instanceRemark;

                model.IssuedByProcessId = instance.IssuedByProcessId.ToGuid();
                model.IssuedByEnterpriseStageId = (await ProcessService.FindAsync(instance.IssuedByProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).EnterpriseStageId;

                model.IssuedToProcessId = instance.IssuedToProcessId.ToGuid();
                model.IssuedToEnterpriseStageId = (await ProcessService.FindAsync(instance.IssuedToProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).EnterpriseStageId;

                model.EmployeeId = instance.EmployeeId.ToGuid();
                model.RefNo = instance.RefNo;
                model.JobType = instance.JobType;
                model.Date = ControllerHelper.ConvertDateTimeToUtc(instance.Date, User.TimeZone());

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Name", "RefNo", "ShrtDesc", "JobType", "IssuedByEnterpriseStageId", "IssuedByProcessId", "IssuedToEnterpriseStageId", "IssuedToProcessId", "EmployeeId", "Remark", "Authorized" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { id = model.Id });
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

        // GET: /StockIssue/Delete/5
        [EsAuthorize(Roles = "StockAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            StockIssueHead model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            model.AspNetUser = ControllerHelper.GetAspNetUser(model.CreatedBy); //Created
            model.AspNetUser1 = ControllerHelper.GetAspNetUser(model.ModifiedBy); // Modified

            model.Employee = await EmployeeService.FindAsync(model.EmployeeId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.EnterpriseStage = await EnterpriseStageService.FindAsync(model.IssuedByEnterpriseStageId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Process = await ProcessService.FindAsync(model.IssuedByProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.EnterpriseStage1 = await EnterpriseStageService.FindAsync(model.IssuedToEnterpriseStageId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Process1 = await ProcessService.FindAsync(model.IssuedToProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            //Always be Stock for stock module
            ViewBag.IssuedByEnterpriseStageId = model.IssuedByEnterpriseStageId;
            ViewBag.IssuedByEnterpriseStageName = (await EnterpriseStageService.FindAsync(model.IssuedByEnterpriseStageId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            ViewBag.IssuedByProcessId = model.IssuedByProcessId;
            ViewBag.IssuedByProcessName = (await ProcessService.FindAsync(model.IssuedByProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            ViewBag.Id = model.Id;
            ViewBag.Name = model.Name;

            return View(model);
        }

        // POST: /StockIssue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "StockAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    StockIssueHead model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    var productLineItems = StockProductIssueService.SelectLineItems(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    var itemLineItems = StockItemIssueService.SelectLineItems(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    string logEntry = "Deleted StockIssue: Id=" + id + " Date=" + model.Date + " Name=" + model.Name + " RefNo=" + model.RefNo + " EmployeeId=" + model.EmployeeId;
                    itemLineItems.ForEach(p => logEntry += "\n LineItems: Id=" + p.Id + " StockIssueId=" + p.StockIssueId + " ItemId=" + p.ItemId + " Quantity=" + p.Quantity + " JobNo=" + p.JobNo);
                    productLineItems.ForEach(p => logEntry += "\n LineItems: Id=" + p.Id + " StockIssueId=" + p.StockIssueId + " ProductId=" + p.ProductId + " Quantity=" + p.Quantity + " JobNo=" + p.JobNo);
                    
                    _service.Delete(model);
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, logEntry);
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
                 records.Where(x => x.Name.ToLower().Contains(searchParam.ToLower()) || x.RefNo.ToLower().Contains(searchParam.ToLower()))
                    .OrderByDescending(x => x.ModifiedDate)
                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
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
        }

        #endregion Handle Exception
    }
}
