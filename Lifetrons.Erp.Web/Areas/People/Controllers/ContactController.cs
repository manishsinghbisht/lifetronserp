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

namespace Lifetrons.Erp.People.Controllers
{
    [EsAuthorize(Roles = "PeopleAuthorize, PeopleEdit, PeopleView")]
    public class ContactController : BaseController
    {
        private readonly IContactService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;

        private readonly ILeadSourceService _leadSourceService;
        private readonly ILevelService _levelService;
        private readonly IAccountService _accountService;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IAddressService _addressService;
        private readonly ITaskService _taskService;

        public ContactController(IContactService service, IUnitOfWorkAsync unitOfWork,
            ILeadSourceService leadSourceService, ILevelService levelService, IAccountService accountService, ITaskService taskService,
            IAspNetUserService aspNetUserService, IAddressService addressService)
        {
            _service = service;
            _unitOfWork = unitOfWork;

            _leadSourceService = leadSourceService;
            _levelService = levelService;
            _accountService = accountService;
            _aspNetUserService = aspNetUserService;
            _addressService = addressService;
            _taskService = taskService;
        }

        // GET: /Contact/
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderBy(x => x.Name).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: /Contact/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            Lifetrons.Erp.Data.Contact instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy);
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy);
            instance.AspNetUser2 = ControllerHelper.GetAspNetUser(instance.OwnerId);
            instance.AspNetUser3 = ControllerHelper.GetAspNetUser(instance.PreferredOwnerId);

            instance.Account = await _accountService.FindAsync(instance.AccountId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.LeadSource = await _leadSourceService.FindAsync(instance.LeadSourceId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Level = await _levelService.FindAsync(instance.LevelId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Contact2 = await _service.FindAsync(instance.ReportsTo.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            return View(instance);
        }

        // GET: /Contact/Create
        [EsAuthorize(Roles = "PeopleAuthorize, PeopleEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.PreferredOwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.AccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.MailingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.OtherAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            ViewBag.LeadSourceId = new SelectList(await _leadSourceService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.LevelId = new SelectList(await _levelService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ReportsTo = new SelectList(await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        // POST: /Contact/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "PeopleAuthorize, PeopleEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,NamePrefix,FirstName,MiddleName,LastName,Code,ShrtDesc,Remark,AccountId,OwnerId,PreferredOwnerId,Title,Department,ReportsTo,PrimaryPhone,PrimaryEMail,CompanyName,LeadSourceId,IsEmployee,IsProspect,IsEndorsement,LevelId,Birthdate,MailingAddressId,MailingAddressToName,MailingAddressLine1,MailingAddressLine2,MailingAddressLine3,MailingAddressCity,MailingAddressPin,MailingAddressState,MailingAddressCountry,MailingAddressPhone,MailingAddressEMail,OtherAddressId,OtherAddressToName,OtherAddressLine1,OtherAddressLine2,OtherAddressLine3,OtherAddressCity,OtherAddressPin,OtherAddressState,OtherAddressCountry,OtherAddressPhone,OtherAddressEMail,LevelId,Birthdate,AnniversaryDate,Authorized")] Contact instance)
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
                instance.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code + Helper.SysSeparator + DateTime.UtcNow;
                instance.Remark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                instance.Birthdate = ControllerHelper.ConvertDateTimeToUtc(instance.Birthdate, User.TimeZone());
                instance.AnniversaryDate = ControllerHelper.ConvertDateTimeToUtc(instance.AnniversaryDate, User.TimeZone());

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

        // GET: /Contact/Edit/5
        [EsAuthorize(Roles = "PeopleAuthorize, PeopleEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Contact instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "PeopleAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("PeopleAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.OwnerId);
            ViewBag.PreferredOwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.PreferredOwnerId);
            ViewBag.AccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.AccountId);
            ViewBag.MailingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.MailingAddressId);
            ViewBag.OtherAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name",  instance.OtherAddressId);

            ViewBag.LeadSourceId = new SelectList(await _leadSourceService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.LeadSourceId);
            ViewBag.LevelId = new SelectList(await _levelService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.LevelId);
            ViewBag.ReportsTo = new SelectList(await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ReportsTo);

            return View(instance);
        }

        // POST: /Contact/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "PeopleAuthorize, PeopleEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,NamePrefix,FirstName,MiddleName,LastName,Code,ShrtDesc,Remark,AccountId,OwnerId,PreferredOwnerId,Title,Department,ReportsTo,PrimaryPhone,PrimaryEMail,CompanyName,LeadSourceId,IsEmployee,IsProspect,IsEndorsement,MailingAddressId,MailingAddressToName,MailingAddressLine1,MailingAddressLine2,MailingAddressLine3,MailingAddressCity,MailingAddressPin,MailingAddressState,MailingAddressCountry,MailingAddressPhone,MailingAddressEMail,OtherAddressId,OtherAddressToName,OtherAddressLine1,OtherAddressLine2,OtherAddressLine3,OtherAddressCity,OtherAddressPin,OtherAddressState,OtherAddressCountry,OtherAddressPhone,OtherAddressEMail,LevelId,Birthdate,AnniversaryDate,Authorized")] Contact instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Contact model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  PeopleAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("PeopleAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Remark = instance.Authorized ? "[Authorized] " + instance.Remark : "[UnAuthorized] " + instance.Remark;
                    }
                }
                //If user is not in role of PeopleAuthorize and record is Authorized, stop editng and show appropriate message.
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
                string instanceRemark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                model.Remark = model.Remark.IsEmpty() ? instanceRemark : model.Remark + "\n" + instanceRemark;

                model.NamePrefix = instance.NamePrefix;
                model.FirstName = instance.FirstName;
                model.MiddleName = instance.MiddleName;
                model.LastName = instance.LastName;
                model.AccountId = instance.AccountId;
                model.OwnerId = instance.OwnerId;
                model.PreferredOwnerId = instance.PreferredOwnerId;
                model.Title = instance.Title;
                model.Department = instance.Department;
                model.ReportsTo = instance.ReportsTo;
                model.PrimaryPhone = instance.PrimaryPhone;
                model.PrimaryEMail = instance.PrimaryEMail;
                model.CompanyName = instance.CompanyName;
                model.LeadSourceId = instance.LeadSourceId;
                model.IsEmployee = instance.IsEmployee;
                model.IsProspect = instance.IsProspect;
                model.IsEndorsement = instance.IsEndorsement;
                model.LevelId = instance.LevelId;
                model.Birthdate = ControllerHelper.ConvertDateTimeToUtc(instance.Birthdate, User.TimeZone());
                model.AnniversaryDate = ControllerHelper.ConvertDateTimeToUtc(instance.AnniversaryDate, User.TimeZone());

                model.MailingAddressId = instance.MailingAddressId.ToGuid();
                model.MailingAddressToName = instance.MailingAddressToName;
                model.MailingAddressLine1 = instance.MailingAddressLine1;
                model.MailingAddressLine2 = instance.MailingAddressLine2;
                model.MailingAddressLine3 = instance.MailingAddressLine3;
                model.MailingAddressCity = instance.MailingAddressCity;
                model.MailingAddressPin = instance.MailingAddressPin;
                model.MailingAddressState = instance.MailingAddressState;
                model.MailingAddressCountry = instance.MailingAddressCountry;
                model.MailingAddressPhone = instance.MailingAddressPhone;
                model.MailingAddressEMail = instance.MailingAddressEMail;

                model.OtherAddressId = instance.OtherAddressId.ToGuid();
                model.OtherAddressToName = instance.OtherAddressToName;
                model.OtherAddressLine1 = instance.OtherAddressLine1;
                model.OtherAddressLine2 = instance.OtherAddressLine2;
                model.OtherAddressLine3 = instance.OtherAddressLine3;
                model.OtherAddressCity = instance.OtherAddressCity;
                model.OtherAddressPin = instance.OtherAddressPin;
                model.OtherAddressState = instance.OtherAddressState;
                model.OtherAddressCountry = instance.OtherAddressCountry;
                model.OtherAddressPhone = instance.OtherAddressPhone;
                model.OtherAddressEMail = instance.OtherAddressEMail;


                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Name", "NamePrefix", "FirstName", "MiddleName", "LastName", "Code", "ShrtDesc", "Remark", "AccountId", "OwnerId", "PreferredOwnerId", "Title", "Department", "ReportsTo", "PrimaryPhone", "PrimaryEMail", "CompanyName", "LeadSourceId", "IsProspect", "IsEndorsement", "MailingAddressId", "MailingAddressToName", "MailingAddressLine1", "MailingAddressLine2", "MailingAddressLine3", "MailingAddressCity", "MailingAddressPin", "MailingAddressState", "MailingAddressCountry", "MailingAddressPhone", "MailingAddressEMail", "OtherAddressId", "OtherAddressToName", "OtherAddressLine1", "OtherAddressLine2", "OtherAddressLine3", "OtherAddressCity", "OtherAddressPin", "OtherAddressState", "OtherAddressCountry", "OtherAddressPhone", "OtherAddressEMail", "LevelId", "Birthdate", "Authorized" };
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

        // GET: /Contact/Delete/5
        [EsAuthorize(Roles = "PeopleAuthorize, PeopleEdit")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Contact model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: /Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "PeopleAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    Contact model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                   model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: ContactId=" + id + " Name=" + model.Name + " Code=" + model.Code);
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
                    .OrderBy(x => x.Name)
                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
        }

        [HttpGet, ActionName("CreateTask")]
        [EsAuthorize(Roles = "PeopleAuthorize, PeopleEdit, PeopleView")]
        public async Task<ActionResult> CreateTask(Guid id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Details", new { id = id });
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            if (applicationUser.OrgId != null) //Only approver is allowed
            {
                Contact model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
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
                    Name = "TCo " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Code = "TCo " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Desc = applicationUser.UserName + ":" + model.Remark,
                    //RelatedToObjectName = "Account",
                    //RelatedToId = model.Id,
                    //LeadId = model.Id,
                    ContactId = model.Id,
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
