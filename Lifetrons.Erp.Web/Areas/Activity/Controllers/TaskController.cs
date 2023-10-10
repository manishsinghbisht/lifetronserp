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
using Microsoft.Ajax.Utilities;
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
using Task = System.Threading.Tasks.Task;

namespace Lifetrons.Erp.Activity.Controllers
{
    [EsAuthorize(Roles = "ServicesAuthorize, ServicesEdit, ServicesView")]
    public class TaskController : BaseController
    {
        private readonly ITaskService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;
       
        private readonly ITaskStatuService _taskStatuService;
        private readonly IContactService _contactService;
        private readonly ILeadService _leadService;
        private readonly IPriorityService _priorityService;
        private readonly IAccountService _accountService;
        private readonly IEmailConfigService _emailConfigService;

        [Dependency]
        public ICampaignService CampaignService { get; set; }

        [Dependency]
        public IOpportunityService OpportunityService { get; set; }

        [Dependency]
        public IQuoteService QuoteService { get; set; }

        [Dependency]
        public IOrderService OrderService { get; set; }

        [Dependency]
        public ICaseService CaseService { get; set; }

        [Dependency]
        public IDispatchService DispatchService { get; set; }

         public TaskController(ITaskService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService, IContactService contactService, 
             ILeadService leadService, ITaskStatuService taskStatuService, IPriorityService priorityService,
             IAccountService accountService, IEmailConfigService emailConfigService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
            _taskStatuService = taskStatuService;
            _contactService = contactService;
            _leadService = leadService;
            _priorityService = priorityService;
            _accountService = accountService;
            _emailConfigService = emailConfigService;
        }

        // GET: /Default1/
         public async Task<ActionResult> Index(int? page)
         {
             var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

             var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
             var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
             var enumerablePage = records.OrderByDescending(x => x.StartDate).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize
             
             return View(enumerablePage);
         }

        // GET: /Default1/Details/5
         public async Task<ActionResult> Details(string id)
         {
             if (id == null)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
             }

             var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

             Lifetrons.Erp.Data.Task instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
             if (instance == null)
             {
                 return HttpNotFound();
             }

             instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
             instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
             instance.AspNetUser2 = ControllerHelper.GetAspNetUser(instance.OwnerId); //Owner
             instance.AspNetUserReportCompletionTo = ControllerHelper.GetAspNetUser(instance.ReportCompletionToId);//Report completition to

             instance.Lead = await _leadService.FindAsync(instance.LeadId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
             instance.Contact = await _contactService.FindAsync(instance.ContactId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
             instance.TaskStatu = await _taskStatuService.FindAsync(instance.TaskStatusId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
             instance.Priority = await _priorityService.FindAsync(instance.PriorityId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
             TempData["RelatedToIdName"] = _service.GetRelatedToIdName(instance.RelatedToObjectName, instance.RelatedToId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

             return View(instance);
         }


        // GET: /Default1/Create
        [EsAuthorize(Roles = "ServicesAuthorize, ServicesEdit")]
         public async Task<ActionResult> Create()
         {
             var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

             ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
             ViewBag.LeadId = new SelectList(await _leadService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
             ViewBag.ContactId = new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
             ViewBag.TaskStatusId = new SelectList(await _taskStatuService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
             ViewBag.PriorityId = new SelectList(await _priorityService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
             ViewBag.ReportCompletionToId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);

             return View();
         }

        // POST: /Default1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ServicesAuthorize, ServicesEdit")]
         public async Task<ActionResult> Create([Bind(Include = "Id,Name,Code,ShrtDesc,Desc,OwnerId,LeadId,ContactId,RelatedToObjectName,RelatedToId,StartDate,EndDate,IsAllDay,TaskStatusId,PriorityId,Reminder,ProgressPercent,ProgressDesc,ReportCompletionToId,Authorized")] Lifetrons.Erp.Data.Task instance)
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
                instance.Code = string.IsNullOrEmpty(instance.Code) ? instance.Name : instance.Code + Helper.SysSeparator + DateTime.UtcNow;

                instance.StartDate = ControllerHelper.ConvertDateTimeToUtc(instance.StartDate, User.TimeZone());
                instance.EndDate = ControllerHelper.ConvertDateTimeToUtc(instance.EndDate, User.TimeZone());
                instance.Reminder = ControllerHelper.ConvertDateTimeToUtc(instance.Reminder, User.TimeZone());

                instance.Desc = instance.Desc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Desc;
                instance.ProgressDesc = instance.ProgressDesc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.ProgressDesc;

                instance.LeadId = instance.LeadId == Guid.Empty ? null : instance.LeadId;
                instance.ContactId = instance.ContactId == Guid.Empty ? null : instance.ContactId;

                if (!instance.RelatedToObjectName.IsNullOrWhiteSpace())
                {
                    instance.RelatedToId = instance.RelatedToId.ToSysGuid();
                }
                
                try
                {
                    _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
                    await _unitOfWork.SaveChangesAsync();
                    await SendUpdateEmail(instance);
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

        // GET: /Default1/Edit/5
        [EsAuthorize(Roles = "ServicesAuthorize, ServicesEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Lifetrons.Erp.Data.Task instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "ServicesAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("ServicesAuthorize"))
            {
                var exception = new ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            await SetupInstanceForm(applicationUser, instance);

            return View(instance);
        }

        private async Task SetupInstanceForm(ApplicationUser applicationUser, Data.Task instance)
        {
            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id",
                "Name", instance.OwnerId);
            ViewBag.LeadId = new SelectList(
                await _leadService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name",
                instance.LeadId);
            ViewBag.ContactId =
                new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.ContactId);
            ViewBag.TaskStatusId =
                new SelectList(await _taskStatuService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.TaskStatusId);
            ViewBag.PriorityId =
                new SelectList(await _priorityService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.PriorityId);
            ViewBag.ReportCompletionToId = new SelectList(
                await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name",
                instance.ReportCompletionToId);
            TempData["RelatedToIdName"] = _service.GetRelatedToIdName(instance.RelatedToObjectName,
                instance.RelatedToId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
        }


        // POST: /Default1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ServicesAuthorize, ServicesEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,ShrtDesc,Desc,OwnerId,LeadId,ContactId,RelatedToObjectName,RelatedToId,StartDate,EndDate,IsAllDay,TaskStatusId,PriorityId,Reminder,ProgressPercent,ProgressDesc,ReportCompletionToId,Authorized")] Lifetrons.Erp.Data.Task instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                bool sendUpdateMail = false;
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Lifetrons.Erp.Data.Task model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  ServicesAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("ServicesAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Desc = instance.Authorized ? "[Authorized] " + instance.Desc : "[UnAuthorized] " + instance.Desc;
                    }
                }
                //If user is not in role of ServicesAuthorize and record is Authorized, stop editng and show appropriate message.
                else if (model.Authorized)
                {
                    TempData["CustomErrorMessage"] = "Authorized record cannot be edited. Please Un-authorize the record to enable editing." +
                                                     "You should have authorization rights to authorize/unauthorize records.";
                    return RedirectToAction("Error", "Error", new { message = TempData["CustomErrorMessage"] });
                }

                //Check if update email need to be sent.
                if (model.OwnerId != instance.OwnerId || model.ReportCompletionToId != instance.ReportCompletionToId)
                {
                    sendUpdateMail = true;
                }

                if (!instance.RelatedToObjectName.IsNullOrWhiteSpace())
                {
                    instance.RelatedToId = instance.RelatedToId.ToSysGuid();
                }

                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;
                model.Authorized = instance.Authorized;

                model.Name = instance.Name;
                model.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code;
                model.ShrtDesc = instance.ShrtDesc;
                string instanceDesc = instance.Desc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Desc;
                model.Desc = model.Desc.IsEmpty() ? instanceDesc : model.Desc + "\n" + instanceDesc;

                model.OwnerId = instance.OwnerId;
                model.LeadId = instance.LeadId == Guid.Empty ? null : instance.LeadId;
                model.ContactId = instance.ContactId == Guid.Empty ? null : instance.ContactId;
                model.RelatedToObjectName = instance.RelatedToObjectName;
                model.RelatedToId = instance.RelatedToId;
                model.StartDate = ControllerHelper.ConvertDateTimeToUtc(instance.StartDate, User.TimeZone());
                model.EndDate = ControllerHelper.ConvertDateTimeToUtc(instance.EndDate, User.TimeZone());
                model.Reminder = ControllerHelper.ConvertDateTimeToUtc(instance.Reminder, User.TimeZone());
                model.IsAllDay = instance.IsAllDay;
                model.TaskStatusId = instance.TaskStatusId;
                model.PriorityId = instance.PriorityId;
                model.ProgressPercent = instance.ProgressPercent;
                string instanceProgressDesc = instance.ProgressDesc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.ProgressDesc;
                model.ProgressDesc = model.ProgressDesc.IsEmpty() ? instanceProgressDesc : model.ProgressDesc + "\n" + instanceProgressDesc;
                model.ReportCompletionToId = instance.ReportCompletionToId;
                
                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Name", "Code", "ShrtDesc", "Desc", "OwnerId", "Authorized", "LeadId", "ContactId", "RelatedToObjectName", "RelatedToId", "StartDate", "EndDate", "IsAllDay", "TaskStatusId", "PriorityId", "Reminder", "ProgressPercent", "ProgressDesc", "ReportCompletionToId" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        //Send update mail
                        if (sendUpdateMail) await SendUpdateEmail(instance);

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

        // GET: /Default1/Delete/5
        [EsAuthorize(Roles = "ServicesAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Lifetrons.Erp.Data.Task model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: /Default1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ServicesAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    Lifetrons.Erp.Data.Task model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                   //if (model.OrgId == applicationUser.OrgId)
                    //{
                    //    _service.Delete(id.ToString());
                    //    await _unitOfWork.SaveChangesAsync();
                    //}

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: TaskId=" + id + " Name=" + model.Name + " Code=" + model.Code);
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

        /// <summary>
        /// This method is called to update dropdown through ajax request
        /// </summary>
        /// <param name="stringifiedParam">Serialized parameter to receive data from client side</param>
        /// <returns>Java script serialized string array to be used with JSON parsing</returns>
        public async virtual Task<JsonResult> ProcessJsonResponseForRelatedToDdl(string stringifiedParam)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //var param = JsonConvert.DeserializeObject(stringifiedParam); //No need of deserialization here as only single value is passed from client
            var param = stringifiedParam;

            var response = new JsonResult();
            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var select = new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            switch (param)
            {
                case "Account":
                    select = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
                    break;
                case "Opportunity":
                    select = new SelectList(await OpportunityService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
                    break;
                case "Order":
                    select = new SelectList(await OrderService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
                    break;
                case "Quote":
                    select = new SelectList(await QuoteService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
                    break;
                case "Campaign":
                    select = new SelectList(await CampaignService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
                    break;
                case "Case":
                    select = new SelectList(await CaseService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
                    break;
                case "Dispatch":
                    select = new SelectList(await DispatchService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
                    break;
            }

            // Create anonymous object
            //var returnData = new
            //{
            //    Item = JsonConvert.SerializeObject(select)
            //};

            // Convert anonymous object to JSON response
            response = Json(select, JsonRequestBehavior.AllowGet);

            // Return JSON
            return response;
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

        private async Task<bool> SendUpdateEmail(Lifetrons.Erp.Data.Task instance)
        {
            try
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                var owner = new AccountController().GetUserById(instance.OwnerId);
                var reportCompletionToUser = new AccountController().GetUserById(instance.ReportCompletionToId);
                string toAddress = owner.AuthenticatedEmail;
                string ccAddress = reportCompletionToUser.AuthenticatedEmail + "," + applicationUser.AuthenticatedEmail;
                string subject = "Task - \"" + instance.Name.Substring(0, instance.Name.Length > 8 ? 8 : instance.Name.Length) + "\" assigned to you!";
                string message = "You may want to look into the Task - \"" + instance.Name + "\" " +
                    "assigned to you by " + applicationUser.FirstName + " " + applicationUser.LastName +
                    ". Please do inform " + reportCompletionToUser.FirstName + " " + reportCompletionToUser.LastName +
                    " on completition.";
                await _emailConfigService.SendMail(applicationUser.Id, applicationUser.OrgId.ToString(), toAddress, ccAddress, subject, message, true);
            }
            catch (Exception exception)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
            }
            return true;
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
            else
            {
                AddErrors(dbUpdateException.Message + " - " + DateTime.UtcNow);
                TempData["CustomErrorMessage"] = dbUpdateException.Message + " - " + dbUpdateException.InnerException.Message;
            }
        }

        #endregion Handle Exception
    }
}
