using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Web.WebPages;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using PagedList;
using Repository;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using Task = System.Threading.Tasks.Task;

namespace Lifetrons.Erp.Activity.Controllers
{
    [EsAuthorize(Roles = "ServicesAuthorize, ServicesEdit, ServicesView")]
    public class CaseController : BaseController
    {
        private readonly ICaseService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IAccountService _accountService;
        private readonly IContactService _contactService;
        private readonly IProductService _productService;
        private readonly ICampaignService _campaignService;
        private readonly IOpportunityService _opportunityService;
        private readonly IQuoteService _quoteService;
        private readonly IOrderService _orderService;
        private readonly IInvoiceService _invoiceService;
        private readonly IPriorityService _priorityService;
        private readonly ICaseReasonService _caseReasonService;
        private readonly ICaseStatusService _caseStatusService;
        private readonly ITaskService _taskService;
        private readonly IEmailConfigService _emailConfigService;

        public CaseController(ICaseService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService,
            IAccountService accountService, IContactService contactService, IProductService productService, ICampaignService campaignService,
            IOpportunityService opportunityService, IQuoteService quoteService, IOrderService orderService, IInvoiceService invoiceService,
            IPriorityService priorityService, ICaseReasonService caseReasonService, ICaseStatusService caseStatusService, ITaskService taskService, IEmailConfigService emailConfigService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
            _accountService = accountService;
            _contactService = contactService;
            _productService = productService;
            _campaignService = campaignService;
            _opportunityService = opportunityService;
            _quoteService = quoteService;
            _orderService = orderService;
            _invoiceService = invoiceService;
            _priorityService = priorityService;
            _caseReasonService = caseReasonService;
            _caseStatusService = caseStatusService;
            _taskService = taskService;
            _emailConfigService = emailConfigService;
        }

        // GET: /Case/
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            IEnumerable<Case> records;

            if (User.IsInRole("AllCases"))
            {
                records = await _service.SelectAsyncAllCases(applicationUser.Id, applicationUser.OrgId.ToString());
            }
            else
            {
                records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            }

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.ModifiedDate).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: /Case/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Case instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            instance.AspNetUser2 = ControllerHelper.GetAspNetUser(instance.OwnerId);  //Owner
            instance.AspNetUserReportCompletionTo = ControllerHelper.GetAspNetUser(instance.ReportCompletionToId);//Report completition to
            instance.AssignedTo = ControllerHelper.GetAspNetUser(instance.AssignedToId);

            instance.Account = await _accountService.FindAsync(instance.AccountId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Contact = await _contactService.FindAsync(instance.ContactId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Product = await _productService.FindAsync(instance.ProductId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Campaign = await _campaignService.FindAsync(instance.CampaignId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Opportunity = await _opportunityService.FindAsync(instance.OpportunityId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Quote = await _quoteService.FindAsync(instance.QuoteId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Order = await _orderService.FindAsync(instance.OrderId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Invoice = await _invoiceService.FindAsync(instance.InvoiceId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Priority = await _priorityService.FindAsync(instance.PriorityId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.CaseReason = await _caseReasonService.FindAsync(instance.CaseReasonId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.CaseStatu = await _caseStatusService.FindAsync(instance.CaseStatusId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.CaseId = instance.Id;
            ViewBag.CaseName = instance.Name;

            return View(instance);
        }

        // GET: /Case/Create
        [EsAuthorize(Roles = "ServicesAuthorize, ServicesEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.AssignedToId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.AccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ContactId = new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ProductId = new SelectList(await _productService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.CampaignId = new SelectList(await _campaignService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.OpportunityId = new SelectList(await _opportunityService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.QuoteId = new SelectList(await _quoteService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.OrderId = new SelectList(await _orderService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.InvoiceId = new SelectList(await _invoiceService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.PriorityId = new SelectList(await _priorityService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.CaseReasonId = new SelectList(await _caseReasonService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.CaseStatusId = new SelectList(await _caseStatusService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ReportCompletionToId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);

            return View();
        }

        // POST: /Case/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ServicesAuthorize, ServicesEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Name,Code,ShrtDesc,Subject,RefNo,Desc,InternalComments,PriorityId,CaseReasonId,OpeningDate,CaseStatusId,OwnerId,AssignedToId,AccountId,ContactId,ProductId,CampaignId,OpportunityId,QuoteId,OrderId,InvoiceId,ReportCompletionToId,CloseDate,ClosingComments,Authorized")] Case instance)
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
                instance.Authorized = false;
                instance.Active = true;
                instance.Name = instance.Name + Helper.SysSeparator + DateTime.UtcNow;
                instance.Code = string.IsNullOrEmpty(instance.Code) ? instance.Name : instance.Code + Helper.SysSeparator + DateTime.UtcNow;
                instance.Desc = instance.Desc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Desc;
                instance.InternalComments = instance.InternalComments.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.InternalComments;

                instance.ReportCompletionToId = instance.ReportCompletionToId; 
                instance.AssignedToId = instance.OwnerId; //AssignedTo is for reference of ReportCompletionToId user to know whom he originally assigned the case
                instance.AccountId = instance.AccountId.ToGuid();
                instance.ContactId = instance.ContactId.ToGuid();
                instance.ProductId = instance.ProductId.ToGuid();
                instance.CampaignId = instance.CampaignId.ToGuid();
                instance.OpportunityId = instance.OpportunityId.ToGuid();
                instance.QuoteId = instance.QuoteId.ToGuid();
                instance.OrderId = instance.OrderId.ToGuid();
                instance.InvoiceId = instance.InvoiceId.ToGuid();
                instance.PriorityId = instance.PriorityId.ToGuid();
                instance.CaseReasonId = instance.CaseReasonId.ToGuid();
                instance.CaseStatusId = instance.CaseStatusId.ToGuid();
                instance.OpeningDate = ControllerHelper.ConvertDateTimeToUtc(instance.OpeningDate, User.TimeZone());
                instance.CloseDate = ControllerHelper.ConvertDateTimeToUtc(instance.CloseDate, User.TimeZone());
                
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


        // GET: /Case/Edit/5
        [EsAuthorize(Roles = "ServicesAuthorize, ServicesEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Case instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "ServicesAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("ServicesAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            await SetupInstanceForm(applicationUser, instance);

            return View(instance);
        }

        private async Task SetupInstanceForm(ApplicationUser applicationUser, Case instance)
        {
            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id",
                "Name", instance.OwnerId);
            ViewBag.AssignedToId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id",
                "Name", instance.AssignedTo);
            ViewBag.ReportCompletionToId = new SelectList(
                await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name",
                instance.ReportCompletionToId);
            ViewBag.AccountId =
                new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.AccountId);
            ViewBag.ContactId =
                new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.ContactId);
            ViewBag.ProductId =
                new SelectList(await _productService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.ProductId);
            ViewBag.CampaignId =
                new SelectList(await _campaignService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.CampaignId);
            ViewBag.OpportunityId =
                new SelectList(await _opportunityService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.OpportunityId);
            ViewBag.QuoteId =
                new SelectList(await _quoteService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.QuoteId);
            ViewBag.OrderId =
                new SelectList(await _orderService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.OrderId);
            ViewBag.InvoiceId =
                new SelectList(await _invoiceService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.InvoiceId);
            ViewBag.PriorityId =
                new SelectList(await _priorityService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.PriorityId);
            ViewBag.CaseReasonId =
                new SelectList(await _caseReasonService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.CaseReasonId);
            ViewBag.CaseStatusId =
                new SelectList(await _caseStatusService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.CaseStatusId);
        }

        // POST: /Case/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ServicesAuthorize, ServicesEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,ShrtDesc,Subject,RefNo,Desc,InternalComments,PriorityId,CaseReasonId,OpeningDate,CaseStatusId,OwnerId,AssignedToId,AccountId,ContactId,ProductId,CampaignId,OpportunityId,QuoteId,OrderId,InvoiceId,ReportCompletionToId,CloseDate,ClosingComments,Authorized,TimeStamp")] Case instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                bool sendUpdateMail = false;
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Case model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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
                model.ReportCompletionToId = instance.ReportCompletionToId;//Keeping AssignedTo and OwnerId same. Has complication in Dashboard and reports
                model.AssignedToId = instance.OwnerId; //AssignedTo is for reference of ReportCompletionToId user to know whom he originally assigned the case
                model.Subject = instance.Subject;
                model.RefNo = instance.RefNo;
                model.AccountId = instance.AccountId.ToGuid();
                model.ContactId = instance.ContactId.ToGuid();
                model.ProductId = instance.ProductId.ToGuid();
                model.CampaignId = instance.CampaignId.ToGuid();
                model.OpportunityId = instance.OpportunityId.ToGuid();
                model.QuoteId = instance.QuoteId.ToGuid();
                model.OrderId = instance.OrderId.ToGuid();
                model.InvoiceId = instance.InvoiceId.ToGuid();
                string instanceInternalComments = instance.InternalComments.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.InternalComments;
                model.InternalComments = model.InternalComments.IsEmpty() ? instanceInternalComments : model.InternalComments + "\n" + instanceInternalComments;
                model.PriorityId = instance.PriorityId.ToGuid();
                model.CaseReasonId = instance.CaseReasonId.ToGuid();
                model.OpeningDate = ControllerHelper.ConvertDateTimeToUtc(instance.OpeningDate, User.TimeZone());
                model.CaseStatusId = instance.CaseStatusId.ToGuid();
                model.CloseDate = ControllerHelper.ConvertDateTimeToUtc(instance.CloseDate, User.TimeZone());
                model.ClosingComments = instance.ClosingComments;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Name", "Code", "ShrtDesc", "Subject", "RefNo", "Desc", "InternalComments", "PriorityId", "CaseReasonId", "OpeningDate", "CaseStatusId", "OwnerId", "AssignedToId", "AccountId", "ContactId", "ProductId", "CampaignId", "OpportunityId", "QuoteId", "OrderId", "InvoiceId", "ReportCompletionToId", "CloseDate", "ClosingComments", "Authorized" };
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

        // GET: /Case/Delete/5
        [EsAuthorize(Roles = "ServicesAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Case model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /Case/Delete/5
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
                    Case model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: CaseId=" + id + " Name=" + model.Name + " Code=" + model.Code);
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

            IEnumerable<Case> records;

            if (User.IsInRole("AllCases"))
            {
                records = await _service.SelectAsyncAllCases(applicationUser.Id, applicationUser.OrgId.ToString());
            }
            else
            {
                records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            }
            
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage =
                 records.Where(x => x.Name.ToLower().Contains(searchParam.ToLower()) || x.Code.ToLower().Contains(searchParam.ToLower()))
                    .OrderByDescending(x => x.ModifiedDate)
                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
        }


        [HttpGet, ActionName("CreateTask")]
        [EsAuthorize(Roles = "ServicesAuthorize, ServicesEdit")]
        public async Task<ActionResult> CreateTask(Guid id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Details", new { id = id });
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            if (applicationUser.OrgId != null) //Only approver is allowed
            {
                Case model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                var name = Helper.RemoveSysSeparator(model.Name);
                var task = new Lifetrons.Erp.Data.Task
                {
                    Id = Guid.NewGuid(),
                    OrgId = applicationUser.OrgId.ToSysGuid(),
                    CreatedBy = applicationUser.Id,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = applicationUser.Id,
                    ModifiedDate = DateTime.UtcNow,
                    Active = true,
                    OwnerId = applicationUser.Id,
                    Name = "TC " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Code = "TC " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Desc = applicationUser.UserName + ":" + model.Desc,
                    RelatedToObjectName = "Case",
                    RelatedToId = model.Id,
                    //LeadId = model.LeadId,
                    ContactId = model.ContactId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.UtcNow.AddDays(2),
                    Reminder = DateTime.UtcNow.AddDays(2),
                    ReportCompletionToId = applicationUser.Id,
                    TaskStatusId = Guid.Parse("CE73B734-A4EF-E311-846B-F0921C571FF8"),
                    PriorityId = Guid.Parse("0392B1F1-E302-4279-88EB-B1BA0620278D"),
                };

                _taskService.Create(task, applicationUser.Id, applicationUser.OrgId.ToString());
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction("Details", "Task", new { id = task.Id, area = "Activity" });
            }
            return RedirectToAction("Details", new { id = id });
        }

        private async Task<bool> SendUpdateEmail(Case instance)
        {
            try
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                var owner = new AccountController().GetUserById(instance.OwnerId);
                var reportCompletionToUser = new AccountController().GetUserById(instance.ReportCompletionToId);
                string toAddress = owner.AuthenticatedEmail;
                string ccAddress = reportCompletionToUser.AuthenticatedEmail + "," + applicationUser.AuthenticatedEmail;
                string subject = "Case - \"" + instance.Name.Substring(0, instance.Name.Length > 8 ? 8 : instance.Name.Length) + "\" assigned to you!";
                string message = "You may want to look into the Case - \"" + instance.Name + "\" " +
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
