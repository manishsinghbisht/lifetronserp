using System.Collections.Generic;
using System.Web.WebPages;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
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

namespace Lifetrons.Erp.Works.Controllers
{
    [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit, WorksView")]
    public class WorkOrderController : BaseController
    {
        private readonly IOrderService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IContractService _contractService;
        private readonly IQuoteService _quoteService;
        private readonly IOpportunityService _opportunityService;
        private readonly IAccountService _accountService;
        private readonly IContactService _contactService;
        private readonly IAddressService _addressService;
        private readonly IDeliveryStatuService _deliveryStatuService;
        private readonly IPriorityService _priorityService;
        private readonly ITaskService _taskService;

        public WorkOrderController(IOrderService service, IUnitOfWorkAsync unitOfWork,
           IAspNetUserService aspNetUserService, IOpportunityService opportunityService,
           IAccountService accountService, IContactService contactService, IAddressService addressService,
            IContractService contractService, IQuoteService quoteService, IDeliveryStatuService deliveryStatuService,
            IPriorityService priorityService, ITaskService taskService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
            _quoteService = quoteService;
            _opportunityService = opportunityService;
            _accountService = accountService;
            _contactService = contactService;
            _addressService = addressService;
            _contractService = contractService;
            _deliveryStatuService = deliveryStatuService;
            _priorityService = priorityService;
            _taskService = taskService;
        }

        // GET: /Order/
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            IEnumerable<Order> records;
            if (User.IsInRole("AllOrders"))
            {
                records = await _service.SelectAsyncAllOrders(applicationUser.Id, applicationUser.OrgId.ToString());
            }
            else
            {
                records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            }

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.CreatedDate).ToPagedList(pageNumber, 5); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: /Order/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //ProductType instance = await _service.FindAsync(id);
            Order instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            instance.AspNetUser2 = ControllerHelper.GetAspNetUser(instance.OwnerId);  //Owner
            instance.AspNetUserActivatedBy = ControllerHelper.GetAspNetUser(instance.OwnerId);
            instance.AspNetUserCmpSignedBy = ControllerHelper.GetAspNetUser(instance.OwnerId);
            instance.AspNetUserReportCompletionTo = ControllerHelper.GetAspNetUser(instance.ReportCompletionToId);//Report completition to

            instance.Account = await _accountService.FindAsync(instance.AccountId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Account1 = await _accountService.FindAsync(instance.SubAccountId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Contact = await _contactService.FindAsync(instance.CustSignedById.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Address = await _addressService.FindAsync(instance.BillingAddressId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Address1 = await _addressService.FindAsync(instance.ShippingAddressId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Opportunity = await _opportunityService.FindAsync(instance.OpportunityId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Quote = await _quoteService.FindAsync(instance.QuoteId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.DeliveryStatu = await _deliveryStatuService.FindAsync(instance.DeliveryStatusId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Priority = await _priorityService.FindAsync(instance.PriorityId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Contract = await _contractService.FindAsync(instance.ContractId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());


            ViewBag.OrderId = instance.Id;
            ViewBag.OrderName = instance.Name;

            return View(instance);
        }

        // GET: /Order/Edit/5
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Order instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
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

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.OwnerId);
            ViewBag.ActivatedById = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.ActivatedById);
            ViewBag.CompanySignedById = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.CompanySignedById);

            ViewBag.AccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.AccountId);
            ViewBag.SubAccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.SubAccountId);
            ViewBag.CustSignedById = new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.CustSignedById);
            ViewBag.OpportunityId = new SelectList(await _opportunityService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.OpportunityId);
            ViewBag.QuoteId = new SelectList(await _quoteService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.QuoteId);
            ViewBag.BillingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.BillingAddressId);
            ViewBag.ShippingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ShippingAddressId);
            ViewBag.DeliveryStatusId = new SelectList(await _deliveryStatuService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.DeliveryStatusId);
            ViewBag.PriorityId = new SelectList(await _priorityService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.PriorityId);
            ViewBag.ContractId = new SelectList(await _contractService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ContractId);
            ViewBag.ReportCompletionToId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.ReportCompletionToId);

            return View(instance);
        }

        // POST: /Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CompanySignedById,StartDate,ActivatedDate,ActivatedById,DeliveryDate,DeliveryStatusId,ProgressPercent,ProgressDesc,Authorized,TimeStamp")] Order instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Order model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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
               
                model.ActivatedById = instance.ActivatedById;
                model.ActivatedDate = ControllerHelper.ConvertDateTimeToUtc(instance.ActivatedDate, User.TimeZone());
                
                model.StartDate = ControllerHelper.ConvertDateTimeToUtc(instance.StartDate, User.TimeZone());
                model.DeliveryDate = ControllerHelper.ConvertDateTimeToUtc(instance.DeliveryDate, User.TimeZone());
                model.DeliveryStatusId = instance.DeliveryStatusId;
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
                        var membersToCompare = new List<string>() { "Name", "Code", "ShrtDesc", "Desc", "OwnerId", "ContractId", "OpportunityId", "QuoteId", "AccountId", "SubAccountId", "RefNo", "PriorityId", "CustSignedById", "CustSignedByDate", "CompanySignedById", "StartDate", "ActivatedDate", "ActivatedById", "DeliveryDate", "DeliveryStatusId", "ProgressPercent", "ProgressDesc", "ReportCompletionToId", "DeliveryTerms", "PaymentTerms", "SpecialTerms", "BillingAddressId", "BillingAddressToName", "BillingAddressLine1", "BillingAddressLine2", "BillingAddressLine3", "BillingAddressCity", "BillingAddressPin", "BillingAddressState", "BillingAddressCountry", "BillingAddressPhone", "BillingAddressEMail", "ShippingAddressId", "ShippingAddressToName", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingAddressLine3", "ShippingAddressCity", "ShippingAddressPin", "ShippingAddressState", "ShippingAddressCountry", "ShippingAddressPhone", "ShippingAddressEMail", "Authorized" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { id = model.Id });
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
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
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException exception)
                {
                    if (exception.InnerException.InnerException.Message.Contains("UQ") ||
                        exception.InnerException.InnerException.Message.Contains("Primary Key") ||
                    exception.InnerException.InnerException.Message.Contains("PK"))
                    {
                        TempData["CustomErrorMessage"] = "Duplicate Name or Code. Key record already exist." + exception.InnerException.InnerException.Message;
                    }
                    else
                    {
                        TempData["CustomErrorMessage"] = exception.InnerException.InnerException.Message;
                    }
                    TempData["CustomErrorDetail"] = exception.InnerException.Message;
                    throw;
                }

                return View(instance);
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

            IEnumerable<Order> records;
            if (User.IsInRole("AllOrders"))
            {
                records = await _service.SelectAsyncAllOrders(applicationUser.Id, applicationUser.OrgId.ToString());
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
        [EsAuthorize(Roles = "WorksPlanner, WorksAuthorize, WorksEdit")]
        public async Task<ActionResult> CreateTask(Guid id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Details", new { id = id });
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            if (applicationUser.OrgId != null) //Only approver is allowed
            {
                Order model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
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
                    Name = "TWo " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Code = "TWo " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Desc = applicationUser.UserName + ":" + model.Remark,
                    RelatedToObjectName = "Order",
                    RelatedToId = model.Id,
                    //LeadId = model.Id,
                    //ContactId = model.ContactId,
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

    }
}
