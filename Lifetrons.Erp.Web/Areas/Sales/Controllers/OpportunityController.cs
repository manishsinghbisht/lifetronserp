using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using PagedList;
using Repository;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using WebGrease.Css.Extensions;
using Task = Lifetrons.Erp.Data.Task;

namespace Lifetrons.Erp.Sales.Controllers
{
    [EsAuthorize(Roles = "SalesAuthorize, SalesEdit, SalesView")]
    public class OpportunityController : BaseController
    {
        [Dependency]
        public IOpportunityLineItemService OpportunityLineItemService { get; set; }

        private readonly IOpportunityService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        private readonly IOpportunityTypeService _opportunityTypeService;
        private readonly IAccountService _accountService;
        private readonly IContactService _contactService;
        private readonly ILeadService _leadService;
        private readonly ICampaignService _campaignService;
        private readonly ILeadSourceService _leadSourceService;
        private readonly IDeliveryStatuService _deliveryStatuService;
        private readonly IStageService _stageService;
        private readonly ITaskService _taskService;

        public OpportunityController(IOpportunityService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService,
            IAccountService accountService, IContactService contactService,
            IOpportunityTypeService opportunityTypeService, ICampaignService campaignService,
            ILeadService leadService, ILeadSourceService leadSourceService,
            IStageService stageService, IDeliveryStatuService deliveryStatuService, ITaskService taskService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
            _accountService = accountService;
            _contactService = contactService;
            _opportunityTypeService = opportunityTypeService;
            _campaignService = campaignService;
            _leadService = leadService;
            _leadSourceService = leadSourceService;
            _stageService = stageService;
            _deliveryStatuService = deliveryStatuService;
            _taskService = taskService;
        }

        // GET: /Opportunity/
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.CreatedDate).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: /Opportunity/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            Opportunity instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = await _aspNetUserService.FindAsync(instance.CreatedBy); //Created
            instance.AspNetUser1 = await _aspNetUserService.FindAsync(instance.ModifiedBy); // Modified
            instance.AspNetUser2 = await _aspNetUserService.FindAsync(instance.OwnerId); //Owner

            instance.LeadSource = await _leadSourceService.FindAsync(instance.LeadSourceId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Campaign = await _campaignService.FindAsync(instance.CampaignId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Account = await _accountService.FindAsync(instance.AccountId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Contact = await _contactService.FindAsync(instance.ContactId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Lead = await _leadService.FindAsync(instance.LeadId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.DeliveryStatu = await _deliveryStatuService.FindAsync(instance.DeliveryStatusId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Stage = await _stageService.FindAsync(instance.StageId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.OpportunityType = await _opportunityTypeService.FindAsync(instance.OpportunityTypeId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.OpportunityId = instance.Id;
            ViewBag.OpportunityName = instance.Name;

            return View(instance);
        }

        // GET: /Opportunity/Create
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.LeadSourceId = new SelectList(await _leadSourceService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.CampaignId = new SelectList(await _campaignService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.AccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ContactId = new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.LeadId = new SelectList(await _leadService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.DeliveryStatusId = new SelectList(await _deliveryStatuService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.StageId = new SelectList(await _stageService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.OpportunityTypeId = new SelectList(await _opportunityTypeService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        // POST: /Opportunity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Code,ShrtDesc,Remark,OwnerId,Private,CampaignId,LeadId,ContactId,AccountId,OpportunityTypeId,LeadSourceId,RefNo,OrderNo,NumberOfEmployees,ExpectedRevenue,CloseDate,NextStep,StageId,ProbabilityPercent,DeliveryStatusId,Competitors,Authorized")] Opportunity instance)
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
                instance.Remark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                instance.AccountId = instance.AccountId.ToGuid();
                instance.ContactId = instance.ContactId.ToGuid();
                instance.CampaignId = instance.CampaignId.ToGuid();
                instance.LeadId = instance.LeadId.ToGuid();
                instance.LeadSourceId = instance.LeadSourceId.ToGuid();
                instance.DeliveryStatusId = instance.DeliveryStatusId.ToGuid();
                instance.StageId = instance.StageId.ToGuid();
                instance.Name = instance.Name + Helper.SysSeparator + DateTime.UtcNow;
                instance.Code = string.IsNullOrEmpty(instance.Code) ? instance.Name : instance.Code + Helper.SysSeparator + DateTime.UtcNow;
                instance.CloseDate = ControllerHelper.ConvertDateTimeToUtc(instance.CloseDate, User.TimeZone());
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

            return RedirectToAction("Details", new { id = instance.Id });
        }

        // GET: /Opportunity/Edit/5
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Opportunity instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
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

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.OwnerId);
            ViewBag.LeadSourceId = new SelectList(await _leadSourceService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.LeadSourceId);
            ViewBag.CampaignId = new SelectList(await _campaignService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.CampaignId);
            ViewBag.AccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.AccountId);
            ViewBag.ContactId = new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ContactId);
            ViewBag.LeadId = new SelectList(await _leadService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.LeadId);
            ViewBag.DeliveryStatusId = new SelectList(await _deliveryStatuService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.DeliveryStatusId);
            ViewBag.StageId = new SelectList(await _stageService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.StageId);
            ViewBag.OpportunityTypeId = new SelectList(await _opportunityTypeService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.OpportunityTypeId);

            return View(instance);
        }

        // POST: /Opportunity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,ShrtDesc,Remark,OwnerId,Private,CampaignId,LeadId,ContactId,AccountId,OpportunityTypeId,LeadSourceId,RefNo,OrderNo,NumberOfEmployees,ExpectedRevenue,CloseDate,NextStep,StageId,ProbabilityPercent,DeliveryStatusId,Competitors,Authorized,TimeStamp")] Opportunity instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Opportunity model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  SalesAuthorize role, process editing. Also update status in Remark field.
                if (User.IsInRole("SalesAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Remark = instance.Authorized ? "[Authorized] " + instance.Remark : "[UnAuthorized] " + instance.Remark;
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
                model.Name = instance.Name;
                model.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code;
                model.ShrtDesc = instance.ShrtDesc;
                string instanceDesc = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                model.Remark = model.Remark.IsEmpty() ? instanceDesc : model.Remark + "\n" + instanceDesc;

                model.OwnerId = instance.OwnerId;
                model.Private = instance.Private;
                model.CampaignId = instance.CampaignId.ToGuid();
                model.LeadId = instance.LeadId.ToGuid();
                model.ContactId = instance.ContactId.ToGuid();
                model.AccountId = instance.AccountId.ToGuid();
                model.OpportunityTypeId = instance.OpportunityTypeId.ToGuid();
                model.LeadSourceId = instance.LeadSourceId.ToGuid();
                model.RefNo = instance.RefNo;
                model.OrderNo = instance.OrderNo;
                model.NumberOfEmployees = instance.NumberOfEmployees;
                model.ExpectedRevenue = instance.ExpectedRevenue;
                model.CloseDate = ControllerHelper.ConvertDateTimeToUtc(instance.CloseDate, User.TimeZone());
                model.NextStep = instance.NextStep;

                model.StageId = instance.StageId.ToGuid();
                model.ProbabilityPercent = instance.ProbabilityPercent;
                model.DeliveryStatusId = instance.DeliveryStatusId.ToGuid();
                model.Competitors = instance.Competitors;
                model.Authorized = instance.Authorized;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Name", "Code", "ShrtDesc", "Remark", "OwnerId", "Private", "CampaignId", "LeadId", "ContactId", "AccountId", "OpportunityTypeId", "LeadSourceId", "RefNo", "OrderNo", "NumberOfEmployees", "ExpectedRevenue", "CloseDate", "NextStep", "StageId", "ProbabilityPercent", "DeliveryStatusId", "Competitors", "Authorized" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { id = model.Id });
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

        // GET: /Opportunity/Delete/5
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Opportunity model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: /Opportunity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) 
                {
                    Opportunity model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    var lineItems = OpportunityLineItemService.SelectLineItems(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    string logEntry = "Deleted: OpportunityId=" + id + " OpportunityNo=" + model.OpportunityNo + " Name=" + model.Name + " Code=" + model.Code;
                    lineItems.ForEach(p => logEntry += "\n LineItems: Id=" + p.Id + "OpportunityId=" + p.OpportunityId + "ProductId=" + p.ProductId + "Quantity=" + p.Quantity);

                    _service.Delete(model);

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
                 records.Where(x => x.Name.ToLower().Contains(searchParam.ToLower()) || x.Code.ToLower().Contains(searchParam.ToLower()))
                    .OrderByDescending(x => x.ModifiedDate)
                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
        }

        [HttpGet, ActionName("CreateTask")]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> CreateTask(Guid id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Details", new { id = id });
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            if (applicationUser.OrgId != null) //Only approver is allowed
            {
                Opportunity model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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
                    Name = "TOp " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Code = "TOp " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Desc = applicationUser.UserName + ":" + model.Remark,
                    RelatedToObjectName = "Opportunity",
                    RelatedToId = model.Id,
                    LeadId = model.LeadId,
                    ContactId = model.ContactId,
                    StartDate = DateTime.UtcNow,
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
