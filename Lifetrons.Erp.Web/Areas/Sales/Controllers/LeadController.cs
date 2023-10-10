using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Microsoft.Ajax.Utilities;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.Sales.Controllers
{
    using Lifetrons.Erp.Controllers;
    using Lifetrons.Erp.Data;
    using Lifetrons.Erp.Helpers;
    using Lifetrons.Erp.Service;
    using Microsoft.AspNet.Identity;
    using Microsoft.Practices.Unity;
    using PagedList;
    using Repository;
    using System;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.WebPages;

    [EsAuthorize(Roles = "SalesAuthorize, SalesEdit, SalesView")]
    public class LeadController : BaseController
    {
        private readonly ILeadService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly ICampaignService _campaignService;
        private readonly ILeadSourceService _leadSourceService;
        private readonly ILeadStatuService _leadStatuService;
        private readonly IRatingService _ratingService;
        private readonly IAddressService _addressService;
        private readonly ITaskService _taskService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IContactService _contactService;

        [Dependency]
        public IStoredProcedureService StoredProcedureService { get; set; }

        public LeadController(ILeadService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService, ICampaignService campaignService, ILeadSourceService leadSourceService,
             ILeadStatuService leadStatuService, IRatingService ratingService, IAddressService addressService, ITaskService taskService, IEmailConfigService emailConfigService, IContactService contactService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
            _campaignService = campaignService;
            _leadSourceService = leadSourceService;
            _leadStatuService = leadStatuService;
            _ratingService = ratingService;
            _addressService = addressService;
            _taskService = taskService;
            _emailConfigService = emailConfigService;
            _contactService = contactService;
        }

        // GET: /Lead/
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.ModifiedDate).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }


        // GET: /Lead/Convert/5
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Convert(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Lead model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            model.AspNetUser = ControllerHelper.GetAspNetUser(model.CreatedBy); //Created
            model.AspNetUser1 = ControllerHelper.GetAspNetUser(model.ModifiedBy); // Modified
            model.AspNetUser2 = ControllerHelper.GetAspNetUser(model.OwnerId); //Owner

            model.LeadSource = await _leadSourceService.FindAsync(model.LeadSourceId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.LeadStatu = await _leadStatuService.FindAsync(model.LeadStatusId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Rating = await _ratingService.FindAsync(model.RatingId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Campaign = await _campaignService.FindAsync(model.CampaignId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Address = await _addressService.FindAsync(model.AddressId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            return View(model);
        }

        // POST: /Lead/Delete/5
        [HttpPost, ActionName("Convert")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Convert(Guid id, bool withContact)
        {
            var db = new Entities();
            var returnValueParameter = new ObjectParameter("returnValue", typeof(int));
            var result = db.spConvertLeadToTasknOpp(id, returnValueParameter);
            if (result == -1)
            {
                return RedirectToAction("Error", "Error", new { message = "Lead Conversion failed." });
            }

            if (withContact)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Lead model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                var contacts = await _contactService.FindAsyncByName(applicationUser.Id, applicationUser.OrgId.ToString(), model.Name);
                if (contacts.Any()) return RedirectToAction("Details", new {id = id});
                try
                {
                        var fullName = model.Name.Trim();
                        fullName = Helper.RemoveSysSeparator(fullName);
                        
                        var firstSpaceIndex = fullName.IndexOf(" ");
                        var firstName = fullName.Substring(0, firstSpaceIndex);
    
                        var lastSpaceIndex = fullName.LastIndexOf(" ");
                        var lastName = fullName.Substring(lastSpaceIndex + 1);

                        var middleName = fullName.Substring(firstSpaceIndex + 1, lastSpaceIndex - firstSpaceIndex);

                    var contact = new Contact()
                    {
                        Id = Guid.NewGuid(),
                        OrgId = applicationUser.OrgId.ToSysGuid(),
                        CreatedBy = applicationUser.Id,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedBy = applicationUser.Id,
                        ModifiedDate = DateTime.UtcNow,
                        Active = true,
                        OwnerId = applicationUser.Id,
                        Name = fullName,
                        Code = fullName + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                        FirstName = firstName,
                        MiddleName = middleName,
                        LastName = lastName,
                        Remark = applicationUser.UserName + ": Converted from Lead on " + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone) + ". " + model.Desc,
                        LeadSourceId = Guid.Parse("20FEC5C0-6BB9-42B6-953E-CCA023C7960C"),
                        LevelId = Guid.Parse("EF2EC456-168C-4190-BB25-4BC87ED34BE5"),
                        PrimaryEMail = model.PrimaryEMail,
                        PrimaryPhone = model.PrimaryPhone,

                        MailingAddressToName = model.AddressToName,
                        MailingAddressLine1 = model.AddressLine1,
                        MailingAddressLine2 = model.AddressLine2,
                        MailingAddressLine3 = model.AddressLine3,
                        MailingAddressCity = model.AddressCity,
                        MailingAddressState = model.AddressState,
                        MailingAddressPin = model.AddressPin,
                        MailingAddressCountry = model.AddressCountry,
                        MailingAddressPhone = model.AddressPhone,
                        MailingAddressEMail = model.AddressEMail

                    };

                    _contactService.Create(contact, applicationUser.Id, applicationUser.OrgId.ToString());
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception exception)
                {
                    Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
                }
               
            }
            return RedirectToAction("Details", new { id = id });
        }


        // GET: /Lead/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            Lead instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            instance.AspNetUser2 = ControllerHelper.GetAspNetUser(instance.OwnerId); //Owner

            instance.LeadSource = await _leadSourceService.FindAsync(instance.LeadSourceId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.LeadStatu = await _leadStatuService.FindAsync(instance.LeadStatusId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Rating = await _ratingService.FindAsync(instance.RatingId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Campaign = await _campaignService.FindAsync(instance.CampaignId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Address = await _addressService.FindAsync(instance.AddressId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            return View(instance);
        }

        // GET: /Lead/Create
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.LeadStatusId = new SelectList(await _leadStatuService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.LeadSourceId = new SelectList(await _leadSourceService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.CampaignId = new SelectList(await _campaignService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.RatingId = new SelectList(await _ratingService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.AddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        // POST: /Lead/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Code,ShrtDesc,Desc,OwnerId,PrimaryPhone,PrimaryEMail,CompanyName,Department,Title,AnnualRevenue,NumberOfEmployees,LeadSourceId,LeadStatusId,RatingId,AddressId,AddressToName,AddressLine1,AddressLine2,AddressLine3,AddressCity,AddressPin,AddressState,AddressCountry,AddressPhone,AddressEMail,Authorized,CampaignId,IsConverted")] Lead instance)
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
                instance.RatingId = instance.RatingId.ToGuid();
                instance.CampaignId = instance.CampaignId.ToGuid();

                instance.Name = instance.Name + Helper.SysSeparator + DateTime.UtcNow;
                instance.Code = string.IsNullOrEmpty(instance.Code) ? instance.Name : instance.Code + Helper.SysSeparator + DateTime.UtcNow;

                instance.Desc = instance.Desc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Desc;
                try
                {
                    _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
                    await _unitOfWork.SaveChangesAsync();
                    await SendUpdateEmail(instance);
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

        // GET: /Lead/Edit/5
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Lead instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }
            if (instance.IsConverted)
            {
                return RedirectToAction("Error", "Error", new { message = "Lead is already converted. Editing not allowed." });
            }

            //If instance.Authorized, only user with "SalesAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("SalesAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.OwnerId);
            ViewBag.LeadStatusId = new SelectList(await _leadStatuService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.LeadStatusId);
            ViewBag.LeadSourceId = new SelectList(await _leadSourceService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.LeadSourceId);
            ViewBag.CampaignId = new SelectList(await _campaignService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.CampaignId);
            ViewBag.RatingId = new SelectList(await _ratingService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.RatingId);
            ViewBag.AddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.AddressId);

            return View(instance);
        }

        // POST: /Lead/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,ShrtDesc,Desc,OwnerId,PrimaryPhone,PrimaryEMail,CompanyName,Department,Title,AnnualRevenue,NumberOfEmployees,LeadSourceId,LeadStatusId,RatingId,AddressId,AddressToName,AddressLine1,AddressLine2,AddressLine3,AddressCity,AddressPin,AddressState,AddressCountry,AddressPhone,AddressEMail,Authorized,CampaignId,IsConverted,TimeStamp")] Lead instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                bool sendUpdateMail = false;
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Lead model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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

                //Check if update email need to be sent.
                if (model.OwnerId != instance.OwnerId)
                {
                    sendUpdateMail = true;
                }

                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;

                model.Name = instance.Name;
                model.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code;
                model.ShrtDesc = instance.ShrtDesc;

                string instanceDesc = instance.Desc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Desc;
                model.Desc = model.Desc.IsEmpty() ? instanceDesc : model.Desc + "\n" + instanceDesc;

                model.OwnerId = instance.OwnerId;
                model.PrimaryPhone = instance.PrimaryPhone;
                model.PrimaryEMail = instance.PrimaryEMail;
                model.CompanyName = instance.CompanyName;
                model.Department = instance.Department;
                model.Title = instance.Title;
                model.AnnualRevenue = instance.AnnualRevenue;
                model.NumberOfEmployees = instance.NumberOfEmployees;
                model.LeadSourceId = instance.LeadSourceId;
                model.LeadStatusId = instance.LeadStatusId;
                model.RatingId = instance.RatingId;
                model.AddressId = instance.AddressId;
                model.CampaignId = instance.CampaignId.ToGuid();
                model.IsConverted = instance.IsConverted;

                model.AddressId = instance.AddressId.ToGuid();
                model.AddressToName = instance.AddressToName;
                model.AddressLine1 = instance.AddressLine1;
                model.AddressLine2 = instance.AddressLine2;
                model.AddressLine3 = instance.AddressLine3;
                model.AddressCity = instance.AddressCity;
                model.AddressPin = instance.AddressPin;
                model.AddressState = instance.AddressState;
                model.AddressCountry = instance.AddressCountry;
                model.AddressPhone = instance.AddressPhone;
                model.AddressEMail = instance.AddressEMail;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Name", "Code", "ShrtDesc", "Desc", "OwnerId", "PrimaryPhone", "PrimaryEMail", "CompanyName", "Department", "Title", "AnnualRevenue", "NumberOfEmployees", "LeadSourceId", "LeadStatusId", "RatingId", "AddressId", "AddressToName", "AddressLine1", "AddressLine2", "AddressLine3", "AddressCity", "AddressPin", "AddressState", "AddressCountry", "AddressPhone", "AddressEMail", "Authorized", "CampaignId", "IsConverted" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        //Send update mail
                        if (sendUpdateMail) await SendUpdateEmail(instance);

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

        // GET: /Lead/Delete/5
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Lead model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: /Lead/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    Lead model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();
                    
                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: LeadId=" + id + " Name=" + model.Name + " Code=" + model.Code);
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
        /// This method is called to update address through ajax request
        /// </summary>
        /// <param name="stringifiedParam">Serialized parameter to receive data from client side</param>
        /// <returns>Java script serialized string array to be used with JSON parsing</returns>
        public virtual JsonResult PostBackAddress(string stringifiedParam)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //var param = JsonConvert.DeserializeObject(stringifiedParam); //No need of deserialization here as only single value is passed from client
            var param = stringifiedParam;

            var response = new JsonResult();
            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            var address = _addressService.Find(param, applicationUser.Id, applicationUser.OrgId.ToString());

            //////// Create anonymous object
            ////var returnData = new
            ////{
            ////    Item = JsonConvert.SerializeObject(address)
            ////};

            // Create anonymous object
            var returnData = new
            {
                AddressToName = address.AddressToName,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                AddressLine3 = address.AddressLine3,
                City = address.City,
                State = address.State,
                Pin = address.PostalCode,
                Country = address.Country,
                Phone = address.Mobile + ", " + address.Phone1,
                EMail = address.AddressToEMail,
            };

            // Convert anonymous object to JSON response
            response = Json(returnData, JsonRequestBehavior.AllowGet);

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

        [HttpGet, ActionName("CreateTask")]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> CreateTask(Guid id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Details", new { id = id });
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            if (applicationUser.OrgId != null) //Only approver is allowed
            {
                Lead model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                var name = model.Name.Substring(0, model.Name.LastIndexOf(Helper.SysSeparator));

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
                    Name = "TL " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Code = "TL " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Desc = applicationUser.UserName + ":" + model.Desc,
                    //RelatedToObjectName = "Quote",
                    //RelatedToId = model.Id,
                    LeadId = model.Id,
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

        private async Task<bool> SendUpdateEmail(Lifetrons.Erp.Data.Lead instance)
        {
            try
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                var owner = new AccountController().GetUserById(instance.OwnerId);
                string toAddress = string.Empty;

                toAddress += instance.PrimaryEMail.IsNullOrWhiteSpace() ? ""
                    : toAddress.IsNullOrWhiteSpace() ? instance.PrimaryEMail : "," + instance.PrimaryEMail;

                toAddress += instance.AddressEMail.IsNullOrWhiteSpace() ? ""
                    : toAddress.IsNullOrWhiteSpace() ? instance.AddressEMail : "," + instance.AddressEMail;

                toAddress += toAddress.IsNullOrWhiteSpace() ? owner.AuthenticatedEmail : "";

                string ccAddress = owner.AuthenticatedEmail + "," + applicationUser.AuthenticatedEmail;
                string subject = "Lead Manager assigned to help you! - \"" + instance.Name.Substring(0, instance.Name.Length > 8 ? 8 : instance.Name.Length) + "\" !";
                string message = "Assigned dedicated manager for you. Please contact to - \"" + owner.FirstName + " " + owner.LastName + " (" + owner.AuthenticatedEmail + ")\" for your queries." +
                    " <br /><br /> Assignment done by " + applicationUser.FirstName + " " + applicationUser.LastName;

                await _emailConfigService.SendMail(applicationUser.Id, applicationUser.OrgId.ToString(), toAddress, ccAddress, subject, message, true);
            }
            catch (Exception exception)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
            }
            return true;
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
