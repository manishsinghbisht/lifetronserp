using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using System.Web.WebPages;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using PagedList;
using Repository;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Sales.Controllers
{
    [EsAuthorize(Roles = "SalesAuthorize, SalesEdit, SalesView")]
    public class CampaignController : BaseController
    {
        private readonly ICampaignService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly ICampaignTypeService _campaignTypeService;
        private readonly ICampaignStatuService _campaignStatuService;
        private readonly ITaskService _taskService;

        public CampaignController(ICampaignService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService, ICampaignTypeService campaignTypeService, ICampaignStatuService campaignStatuService, ITaskService taskService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
            _campaignTypeService = campaignTypeService;
            _campaignStatuService = campaignStatuService;
            _taskService = taskService;
        }

        // GET: /Campaign/
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.ModifiedDate).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: /Campaign/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            Campaign instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy.ToString());
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy.ToString());
            instance.AspNetUser2 = ControllerHelper.GetAspNetUser(instance.OwnerId.ToString());
            instance.CampaignStatu = await _campaignStatuService.FindAsync(instance.CampaignStatusId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.CampaignType = await _campaignTypeService.FindAsync(instance.CampaignTypeId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Campaign2 = await _service.FindAsync(instance.ParentCampaignId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            return View(instance);
        }

        // GET: /Campaign/Create
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.CampaignStatusId = new SelectList(await _campaignStatuService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.CampaignTypeId = new SelectList(await _campaignTypeService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ParentCampaignId = new SelectList(await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        // POST: /Campaign/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create([Bind(Include="Id,Name,Code,ShrtDesc,Desc,OwnerId,NumberOfEmployees,EmployeeDetails,CampaignTypeId,CampaignStatusId,StartDate,EndDate,ExpectedRevenue,BudgetCost,ActualCost,ExpectedResponsePercent,NumSent,ParentCampaignId,Delivery,Competitors,Authorized")] Campaign instance)
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
                instance.ParentCampaignId = instance.ParentCampaignId == Guid.Empty ? null : instance.ParentCampaignId;
                instance.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code;
                instance.Desc = instance.Desc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Desc;

                try
                {
                    _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
                    await _unitOfWork.SaveChangesAsync();
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
            }

            return RedirectToAction("Details", new { id = instance.Id });

        }

        // GET: /Campaign/Edit/5
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Campaign instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
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
            ViewBag.CampaignStatusId = new SelectList(await _campaignStatuService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.CampaignStatusId);
            ViewBag.CampaignTypeId = new SelectList(await _campaignTypeService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.CampaignTypeId);
            ViewBag.ParentCampaignId = new SelectList(await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ParentCampaignId);

            return View(instance);
        }

        // POST: /Campaign/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit([Bind(Include="Id,Name,Code,ShrtDesc,Desc,OwnerId,NumberOfEmployees,EmployeeDetails,CampaignTypeId,CampaignStatusId,StartDate,EndDate,ExpectedRevenue,BudgetCost,ActualCost,ExpectedResponsePercent,NumSent,ParentCampaignId,Delivery,Competitors,Authorized")] Campaign instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Campaign model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  SalesAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("SalesAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Desc = instance.Authorized ? "[Authorized] " + instance.Desc : "[UnAuthorized] " + instance.Desc;
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
                string instanceDesc = instance.Desc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Desc;
                model.Desc = model.Desc.IsEmpty() ? instanceDesc : model.Desc + "\n" + instanceDesc;

                model.OwnerId = instance.OwnerId;
                model.NumberOfEmployees = instance.NumberOfEmployees;
                model.EmployeeDetails = instance.EmployeeDetails;
                model.CampaignTypeId = instance.CampaignTypeId;
                model.CampaignStatusId = instance.CampaignStatusId;
                model.StartDate = instance.StartDate;
                model.EndDate = instance.EndDate;
                model.ExpectedRevenue = instance.ExpectedRevenue;
                model.BudgetCost = instance.BudgetCost;
                model.ActualCost = instance.ActualCost;
                model.ExpectedResponsePercent = instance.ExpectedResponsePercent;
                model.NumSent = instance.NumSent;
                model.ParentCampaignId = instance.ParentCampaignId == Guid.Empty ? null : instance.ParentCampaignId;
                model.CampaignStatusId = instance.CampaignStatusId;
                model.Delivery = instance.Delivery;
                model.Competitors = instance.Competitors;
               
                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

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

        // GET: /Campaign/Delete/5
        [EsAuthorize(Roles = "SalesAuthorize")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Campaign model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: /Campaign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    Campaign model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();
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
                Campaign model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
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
                    Name = "TCp " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Code = "TCp " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Desc = applicationUser.UserName + ":" + model.Desc,
                    RelatedToObjectName = "Campaign",
                    RelatedToId = model.Id,
                    //LeadId = model.ContactId,
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
