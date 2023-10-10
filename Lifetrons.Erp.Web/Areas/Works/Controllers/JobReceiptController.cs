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

namespace Lifetrons.Erp.Works.Controllers
{
    [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit, WorksView")]
    public class JobReceiptController : BaseController
    {
        [Dependency]
        public IEmployeeService EmployeeService { get; set; }

        [Dependency]
        public IEnterpriseStageService EnterpriseStageService { get; set; }

        [Dependency]
        public IProcessService ProcessService { get; set; }

        [Dependency]
        public IJobProductReceiptService JobProductReceiptService { get; set; }

        [Dependency]
        public IJobItemReceiptService JobItemReceiptService { get; set; }

        private readonly IJobReceiptHeadService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        public JobReceiptController(IJobReceiptHeadService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
        }

        // GET: /JobReceipt/
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records =  await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.ModifiedDate).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: /JobReceipt/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            JobReceiptHead instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            
            instance.Employee = await EmployeeService.FindAsync(instance.EmployeeId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Process = await ProcessService.FindAsync(instance.ReceiptByProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Process1 = await ProcessService.FindAsync(instance.ReceiptFromProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            instance.JobProductReceipts = (await JobProductReceiptService.SelectAsyncLineItems(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).ToList();
            instance.JobItemReceipts = (await JobItemReceiptService.SelectAsyncLineItems(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).ToList();

            //Set id to pass on detail partial page
            ViewBag.JobReceiptId = instance.Id;

            return View(instance);
        }

        // GET: /JobReceipt/Create
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.EmployeeId = new SelectList(await EmployeeService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ReceiptByProcessId = new SelectList(await ProcessService.SelectAsyncForJobs(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ReceiptFromProcessId = new SelectList(await ProcessService.SelectAsyncForJobs(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");          
            
            return View();
        }

        // POST: /JobReceipt/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Name,RefNo,JobType,Date,ReceiptByProcessId,ReceiptFromProcessId,EmployeeId,Remark,Authorized")] JobReceiptHead instance)
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

                instance.ReceiptByProcessId = instance.ReceiptByProcessId.ToGuid();
                instance.ReceiptFromProcessId = instance.ReceiptFromProcessId.ToGuid();

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

        // GET: /JobReceipt/Edit/5
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            JobReceiptHead instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
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

            await SetupInstanceForm(applicationUser, instance);

            return View(instance);
        }

        private async Task SetupInstanceForm(ApplicationUser applicationUser, JobReceiptHead instance)
        {
            ViewBag.EmployeeId =
                new SelectList(await EmployeeService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.EmployeeId);
            ViewBag.ReceiptByProcessId =
                new SelectList(await ProcessService.SelectAsyncForJobs(applicationUser.Id, applicationUser.OrgId.ToString()),
                    "Id", "Name", instance.ReceiptByProcessId);
            ViewBag.ReceiptFromProcessId =
                new SelectList(await ProcessService.SelectAsyncForJobs(applicationUser.Id, applicationUser.OrgId.ToString()),
                    "Id", "Name", instance.ReceiptFromProcessId);
        }

        // POST: /JobReceipt/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,RefNo,JobType,Date,ReceiptByProcessId,ReceiptFromProcessId,EmployeeId,Remark,Authorized,TimeStamp")] JobReceiptHead instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                JobReceiptHead model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  WorksAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("WorksAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Remark = instance.Authorized ? "[Authorized] " + instance.Remark : "[UnAuthorized] " + instance.Remark;
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
                model.Name = instance.Name;
                string instanceRemark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                model.Remark = model.Remark.IsEmpty() ? instanceRemark : model.Remark + "\n" + instanceRemark;

                model.ReceiptByProcessId = instance.ReceiptByProcessId.ToGuid();
                model.ReceiptFromProcessId = instance.ReceiptFromProcessId.ToGuid();

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
                        var membersToCompare = new List<string>() { "Name", "RefNo", "ShrtDesc", "JobType", "ReceiptByProcessId", "ReceiptFromProcessId", "EmployeeId", "Date", "Remark", "Authorized" };
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

        // GET: /JobReceipt/Delete/5
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            JobReceiptHead model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            model.AspNetUser = ControllerHelper.GetAspNetUser(model.CreatedBy); //Created
            model.AspNetUser1 = ControllerHelper.GetAspNetUser(model.ModifiedBy); // Modified

            model.Employee = await EmployeeService.FindAsync(model.EmployeeId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Process = await ProcessService.FindAsync(model.ReceiptByProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Process1 = await ProcessService.FindAsync(model.ReceiptFromProcessId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.Id = model.Id;
            ViewBag.Name = model.Name;

            return View(model);
        }

        // POST: /JobReceipt/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    JobReceiptHead model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    var productLineItems = JobProductReceiptService.SelectLineItems(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    var itemLineItems = JobItemReceiptService.SelectLineItems(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    string logEntry = "Deleted JobReceipt: Id=" + id + " Name=" + model.Name + " Date=" + model.Date + " RefNo=" + model.RefNo + " ReceiptByProcessId=" + model.ReceiptByProcessId + " ReceiptFromProcessId=" + model.ReceiptFromProcessId + " EmployeeId=" + model.EmployeeId;
                    productLineItems.ForEach(p => logEntry += "\n JobProductReceipts: Id=" + p.Id + " JobReceiptId=" + p.JobReceiptId + " JobNo=" + p.JobNo + " LabourCharge=" + p.LabourCharge + " Quantity=" + p.Quantity);
                    itemLineItems.ForEach(p => logEntry += "\n JobItemReceipts: Id=" + p.Id + " JobReceiptId=" + p.JobReceiptId + " JobNo=" + p.JobNo + " ItemId=" + p.ItemId + " Quantity=" + p.Quantity);
                    
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
            AddErrors(exceptionMessage);

            return exceptionMessage;
        }

        private void HandleException(Exception exception)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
            TempData["CustomErrorMessage"] = exception.Message;
            TempData["CustomErrorDetail"] = exception.InnerException.Message;
            AddErrors(exception.Message.ToString() + " - " + exception.InnerException.Message.ToString());
        }

        private void AddErrors(string errorMessage)
        {
            ModelState.AddModelError("", errorMessage);
        }

        #endregion Handle Exception
    }
}
