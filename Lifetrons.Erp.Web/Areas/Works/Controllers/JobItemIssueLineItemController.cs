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

namespace Lifetrons.Erp.Works.Controllers
{
    [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit, WorksView")]
    public class JobItemIssueLineItemController : BaseController
    {
        private readonly IJobItemIssueService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;

        private readonly IJobIssueHeadService _jobIssueHeadService;
        private readonly IWeightUnitService _weightUnitService;

        [Dependency]
        public IAspNetUserService AspNetUserService { get; set; }

        [Dependency]
        public IItemService ItemService { get; set; }

        // GET: /JobItemIssueLineItem/
        public JobItemIssueLineItemController(IJobItemIssueService service, IUnitOfWorkAsync unitOfWork, IJobIssueHeadService jobIssueHeadService,
            IWeightUnitService weightUnitService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _jobIssueHeadService = jobIssueHeadService;
            _weightUnitService = weightUnitService;
        }

        // GET: /JobItemIssueLineItem/
        public ActionResult Index(string jobIssueId)
        {
            ViewBag.JobIssueId = jobIssueId;

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = _service.SelectLineItems(jobIssueId, applicationUser.Id, applicationUser.OrgId.ToString());

            return PartialView(model);
        }

        // GET: /JobItemIssueLineItem/Details/5
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
            instance.Item = await ItemService.FindAsync(instance.ItemId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.JobIssueId = instance.JobIssueId;
            ViewBag.JobIssueHeadName = (await _jobIssueHeadService.FindAsync(instance.JobIssueId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).Name;

            return View(instance);
        }

        // GET: /JobItemIssueLineItem/Create
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        public async Task<ActionResult> Create(string jobIssueId)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.WeightUnitId = new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ItemId = new SelectList(await ItemService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            ViewBag.JobIssueId = jobIssueId;
            ViewBag.JobIssueHeadName = (await _jobIssueHeadService.FindAsync(jobIssueId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
            return View();
        }

        // POST: /JobItemIssueLineItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        public async Task<ActionResult> Create([Bind(Include = "JobIssueId,Serial,ItemId,JobNo,Quantity,Weight,WeightUnitId,Rate,Remark,CustomColumn1,Authorized")] JobItemIssue instance)
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
                    return RedirectToAction("Details", new { id = instance.Id.ToString() });
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


        // GET: /JobItemIssueLineItem/Edit/5
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            JobItemIssue instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return HttpNotFound();
            }

            await SetupInstanceForm(applicationUser, instance);

            return View(instance);
        }

        private async Task SetupInstanceForm(ApplicationUser applicationUser, JobItemIssue instance)
        {
            ViewBag.WeightUnitId =
                new SelectList(await _weightUnitService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.WeightUnitId);
            ViewBag.ItemId = new SelectList(
                await ItemService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name",
                instance.ItemId);

            ViewBag.JobIssueId = instance.JobIssueId;
            ViewBag.JobIssueHeadName =
                (await
                    _jobIssueHeadService.FindAsync(instance.JobIssueId.ToString(), applicationUser.Id,
                        applicationUser.OrgId.ToString())).Name;
        }

        // POST: /JobItemIssueLineItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,JobIssueId,Serial,ItemId,JobNo,Quantity,Weight,WeightUnitId,Rate,Remark,CustomColumn1,Authorized,Active,TimeStamp")] JobItemIssue instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                JobItemIssue model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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
                model.JobIssueId = instance.JobIssueId;

                model.Serial = instance.Serial;
                model.ItemId = instance.ItemId.ToGuid();
                model.JobNo = instance.JobNo;
                model.Quantity = instance.Quantity;
                model.Weight = instance.Weight;
                model.WeightUnitId = instance.WeightUnitId;
                model.Rate = instance.Rate;
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
                        var membersToCompare = new List<string>() { "Serial", "ItemId", "JobNo", "Quantity", "Weight", "WeightUnitId", "Rate", "Authorized", "Active" };
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

        // GET: /JobItemIssueLineItem/Delete/5
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize")]
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

            model.JobIssueHead = await _jobIssueHeadService.FindAsync(model.JobIssueId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.JobItemIssueId = model.JobIssueId;
            ViewBag.JobItemIssueHeadName = model.JobIssueHead.Name;

            return View(model);
        }

        // POST: /JobItemIssueLineItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                string parentGuid = string.Empty;
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    JobItemIssue model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    parentGuid = model.JobIssueId.ToString();
                    string logEntry = "Deleted JobItemIssue: Id=" + model.Id + " JobIssueId=" + model.JobIssueId + " JobNo=" + model.JobNo + "  ItemId=" + model.ItemId;
                    _service.Delete(id.ToString());

                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, logEntry);
                }

                return RedirectToAction("Details", "JobIssue", new { id = parentGuid });
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
