using System.Collections.Generic;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using PagedList;
using Repository;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace Lifetrons.Erp.People.Controllers
{
    [EsAuthorize(Roles = "PeopleAuthorize, PeopleEdit, PeopleView")]
    public class SAccountController : BaseController
    {
        private readonly IAccountService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAccountTypeService _accountTypeService;
        private readonly IOwnershipService _ownershipService;
        private readonly IIndustryService _industryService;
        private readonly IRatingService _ratingService;
        private readonly IAddressService _addressService;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly ITaskService _taskService;

        public SAccountController(IAccountService service, IUnitOfWorkAsync unitOfWork, 
            IAccountTypeService accountTypeService, IOwnershipService ownershipService,
            IIndustryService industryService, IRatingService ratingService, 
            IAddressService addressService, ITaskService taskService, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;

            _accountTypeService = accountTypeService;
            _ownershipService = ownershipService;
            _industryService = industryService;
            _ratingService = ratingService;
            _addressService = addressService;
            _taskService = taskService;
            _aspNetUserService = aspNetUserService;
        }

        // GET: /SAccount/
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderBy(x => x.Name).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: /SAccount/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            Account instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy.ToString());
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy.ToString());
            instance.AspNetUser2 = ControllerHelper.GetAspNetUser(instance.OwnerId.ToString());

            instance.AccountType = await _accountTypeService.FindAsync(instance.AccountTypeId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Industry = await _industryService.FindAsync(instance.IndustryId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Ownership = await _ownershipService.FindAsync(instance.OwnershipId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Rating = await _ratingService.FindAsync(instance.RatingId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            return View(instance);
        }

        // GET: /SAccount/Create
        [EsAuthorize(Roles = "PeopleAuthorize, PeopleEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.AccountTypeId = new SelectList(await _accountTypeService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.OwnershipId = new SelectList(await _ownershipService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.IndustryId = new SelectList(await _industryService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.RatingId = new SelectList(await _ratingService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.BillingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ShippingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            
            return View();
        }

        // POST: /SAccount/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "PeopleAuthorize, PeopleEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Code,ShrtDesc,Remark,OwnerId,IsSupplier,Priority,AccountTypeId,IndustryId,OwnershipId,AnnualRevenue,NumberOfEmployees,NumberOfLocations,RatingId,AgreementSerialNumber,AgreementExpDate,BillingAddressId,BillingAddressToName,BillingAddressLine1,BillingAddressLine2,BillingAddressLine3,BillingAddressCity,BillingAddressPin,BillingAddressState,BillingAddressCountry,BillingAddressPhone,BillingAddressEMail,ShippingAddressId,ShippingAddressToName,ShippingAddressLine1,ShippingAddressLine2,ShippingAddressLine3,ShippingAddressCity,ShippingAddressPin,ShippingAddressState,ShippingAddressCountry,ShippingAddressPhone,ShippingAddressEMail,OrgId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Authorized")] Account instance)
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
                           : instance.Code;
                instance.Remark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                instance.AgreementExpDate = ControllerHelper.ConvertDateTimeToUtc(instance.AgreementExpDate, User.TimeZone());

                try
                {
                    _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException exception)
                {
                    if (exception.InnerException.InnerException.Message.Contains("UQ"))
                    {
                        TempData["CustomErrorMessage"] = "Duplicate Name or Code.";
                        TempData["CustomErrorDetail"] = exception.InnerException.Message;
                        throw;
                    }
                }
            }

            //ViewBag.ProductFamilyId = new SelectList(await _productFamilyService.GetAsync(), "Id", "Name");
            //ViewBag.ProductTypeId = new SelectList(await _productTypeService.GetAsync(), "Id", "Name");

            return RedirectToAction("Details", new { id = instance.Id });
        }

        // GET: /SAccount/Edit/5
        [EsAuthorize(Roles = "PeopleAuthorize, PeopleEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Account instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
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
            ViewBag.AccountTypeId = new SelectList(await _accountTypeService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.AccountTypeId);
            ViewBag.OwnershipId = new SelectList(await _ownershipService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.OwnershipId);
            ViewBag.IndustryId = new SelectList(await _industryService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.IndustryId);
            ViewBag.RatingId = new SelectList(await _ratingService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.RatingId);
            ViewBag.BillingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.BillingAddressId);
            ViewBag.ShippingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ShippingAddressId);
            
            return View(instance);
        }

        // POST: /SAccount/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit(AuditingLevel = 1)]
        [EsAuthorize(Roles = "PeopleAuthorize, PeopleEdit")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,ShrtDesc,Remark,OwnerId,IsSupplier,Priority,AccountTypeId,IndustryId,OwnershipId,AnnualRevenue,NumberOfEmployees,NumberOfLocations,RatingId,AgreementSerialNumber,AgreementExpDate,BillingAddressId,BillingAddressToName,BillingAddressLine1,BillingAddressLine2,BillingAddressLine3,BillingAddressCity,BillingAddressPin,BillingAddressState,BillingAddressCountry,BillingAddressPhone,BillingAddressEMail,ShippingAddressId,ShippingAddressToName,ShippingAddressLine1,ShippingAddressLine2,ShippingAddressLine3,ShippingAddressCity,ShippingAddressPin,ShippingAddressState,ShippingAddressCountry,ShippingAddressPhone,ShippingAddressEMail,Authorized")] Account instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Account model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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
                           : instance.Code + Helper.SysSeparator + DateTime.UtcNow;
                model.ShrtDesc = instance.ShrtDesc;
                string instanceRemark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                model.Remark = model.Remark.IsEmpty() ? instanceRemark : model.Remark + "\n" + instanceRemark;
               
                model.OwnerId = instance.OwnerId;
                model.IsSupplier = instance.IsSupplier;
                model.Priority = instance.Priority;
                model.AccountTypeId = instance.AccountTypeId;
                model.IndustryId = instance.IndustryId;
                model.OwnershipId = instance.OwnershipId;
                model.AnnualRevenue = instance.AnnualRevenue;
                model.NumberOfEmployees = instance.NumberOfEmployees;
                model.NumberOfLocations = instance.NumberOfLocations;
                model.RatingId = instance.RatingId;
                model.AgreementSerialNumber = instance.AgreementSerialNumber;
                model.AgreementExpDate = ControllerHelper.ConvertDateTimeToUtc(instance.AgreementExpDate, User.TimeZone());

                model.BillingAddressId = instance.BillingAddressId.ToGuid();
                model.BillingAddressToName = instance.BillingAddressToName;
                model.BillingAddressLine1 = instance.BillingAddressLine1;
                model.BillingAddressLine2 = instance.BillingAddressLine2;
                model.BillingAddressLine3 = instance.BillingAddressLine3;
                model.BillingAddressCity = instance.BillingAddressCity;
                model.BillingAddressPin = instance.BillingAddressPin;
                model.BillingAddressState = instance.BillingAddressState;
                model.BillingAddressCountry = instance.BillingAddressCountry;
                model.BillingAddressPhone = instance.BillingAddressPhone;
                model.BillingAddressEMail = instance.BillingAddressEMail;

                model.ShippingAddressId = instance.ShippingAddressId.ToGuid();
                model.ShippingAddressToName = instance.ShippingAddressToName;
                model.ShippingAddressLine1 = instance.ShippingAddressLine1;
                model.ShippingAddressLine2 = instance.ShippingAddressLine2;
                model.ShippingAddressLine3 = instance.ShippingAddressLine3;
                model.ShippingAddressCity = instance.ShippingAddressCity;
                model.ShippingAddressPin = instance.ShippingAddressPin;
                model.ShippingAddressState = instance.ShippingAddressState;
                model.ShippingAddressCountry = instance.ShippingAddressCountry;
                model.ShippingAddressPhone = instance.ShippingAddressPhone;
                model.ShippingAddressEMail = instance.ShippingAddressEMail;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Name", "Code", "ShrtDesc", "Remark", "OwnerId", "Priority", "IsSupplier", "AccountTypeId", "IndustryId", "OwnershipId", "AnnualRevenue", "NumberOfEmployees", "NumberOfLocations", "RatingId", "AgreementSerialNumber", "AgreementExpDate", "BillingAddressId", "BillingAddressToName", "BillingAddressLine1", "BillingAddressLine2", "BillingAddressLine3", "BillingAddressCity", "BillingAddressPin", "BillingAddressState", "BillingAddressCountry", "BillingAddressPhone", "BillingAddressEMail", "ShippingAddressId", "ShippingAddressToName", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingAddressLine3", "ShippingAddressCity", "ShippingAddressPin", "ShippingAddressState", "ShippingAddressCountry", "ShippingAddressPhone", "ShippingAddressEMail", "Authorized" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

                        return RedirectToAction("Details", new { id = model.Id });
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException exception)
                {
                    if (exception.InnerException.InnerException.Message.Contains("UQ"))
                    {
                        TempData["CustomErrorMessage"] = "Duplicate Name or Code.";
                        TempData["CustomErrorDetail"] = exception.InnerException.Message;
                        throw;
                    }
                }
               
                return View(instance);
            }

            return HttpNotFound();

        }

        // GET: /SAccount/Delete/5
        [EsAuthorize(Roles = "PeopleAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Account model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: /SAccount/Delete/5
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
                    Account model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    
                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString()); //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: AccountId=" + id + " Name=" + model.Name + " Code=" + model.Code);

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
                Account model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
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
                    Name = "TA " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Code = "TA " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Desc = applicationUser.UserName + ":" + model.Remark,
                    RelatedToObjectName = "Account",
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
