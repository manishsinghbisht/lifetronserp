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

namespace Lifetrons.Erp.Sales.Controllers
{
    [EsAuthorize(Roles = "SalesAuthorize, SalesEdit, SalesView")]
    public class ContractController : BaseController
    {
        private readonly IContractService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IQuoteService _quoteService;
        private readonly IOpportunityService _opportunityService;
        private readonly IAccountService _accountService;
        private readonly IContactService _contactService;
        private readonly IAddressService _addressService;
        private readonly IPriceBookService _priceBookService;

        // GET: /Contract/
        public ContractController(IContractService service, IUnitOfWorkAsync unitOfWork, IQuoteService quoteService, 
            IAspNetUserService aspNetUserService, IQuoteStatusService quoteStatusService, IOpportunityService opportunityService,
            IAccountService accountService, IContactService contactService, IAddressService addressService, IPriceBookService priceBookService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
            _quoteService = quoteService;
            _opportunityService = opportunityService;
            _accountService = accountService;
            _contactService = contactService;
            _addressService = addressService;
            _priceBookService = priceBookService;
        }

        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.CreatedDate).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: /Contract/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Contract instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            instance.AspNetUser2 = ControllerHelper.GetAspNetUser(instance.OwnerId);  //Owner
            instance.AspNetUserCmpSignedBy = ControllerHelper.GetAspNetUser(instance.CompanySignedById);  //CmpSignedById

            instance.Account = await _accountService.FindAsync(instance.AccountId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Contact = await _contactService.FindAsync(instance.CustSignedById.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            
            instance.Address = await _addressService.FindAsync(instance.BillingAddressId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Address1 = await _addressService.FindAsync(instance.ShippingAddressId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Opportunity = await _opportunityService.FindAsync(instance.OpportunityId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Quote = await _quoteService.FindAsync(instance.QuoteId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.PriceBook = await _priceBookService.FindAsync(instance.PriceBookId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.ContractId = instance.Id;
            ViewBag.ContractName = instance.Name;

            return View(instance);
        }

        // GET: /Contract/Create
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.AccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.CompanySignedById = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.CustSignedById = new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ContactId = new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.OpportunityId = new SelectList(await _opportunityService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.QuoteId = new SelectList(await _quoteService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.BillingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ShippingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.PriceBookId = new SelectList(await _priceBookService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        // POST: /Contract/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Name,Code,ShrtDesc,Desc,OwnerId,ContractNo,RefNo,OpportunityId,QuoteId,AccountId,PriceBookId,CustSignedById,CustSignedByTitle,CustSignedByDate,ContractStartDate,ContractTenure,ContractExpirationDate,ExpirationNotice,CompanySignedById,CompanySignedDate,DeliveryTerms,PaymentTerms,SpecialTerms,BillingAddressId,BillingAddressToName,BillingAddressLine1,BillingAddressLine2,BillingAddressLine3,BillingAddressCity,BillingAddressPin,BillingAddressState,BillingAddressCountry,BillingAddressPhone,BillingAddressEMail,ShippingAddressId,ShippingAddressToName,ShippingAddressLine1,ShippingAddressLine2,ShippingAddressLine3,ShippingAddressCity,ShippingAddressPin,ShippingAddressState,ShippingAddressCountry,ShippingAddressPhone,ShippingAddressEMail,OrgId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Authorized")] Contract instance)
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
                instance.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code;
                instance.AccountId = instance.AccountId.ToGuid();
                instance.PriceBookId = instance.PriceBookId.ToGuid();
                instance.CustSignedById = instance.CustSignedById.ToGuid();
                instance.CompanySignedById = instance.CompanySignedById;
                instance.QuoteId = instance.QuoteId.ToGuid();
                instance.OpportunityId = instance.OpportunityId.ToGuid();
                instance.BillingAddressId = instance.BillingAddressId.ToGuid();
                instance.ShippingAddressId = instance.ShippingAddressId.ToGuid();
                instance.CustSignedByDate = ControllerHelper.ConvertDateTimeToUtc(instance.CustSignedByDate, User.TimeZone());
                instance.CompanySignedDate = ControllerHelper.ConvertDateTimeToUtc(instance.CompanySignedDate, User.TimeZone());
                instance.ContractStartDate = ControllerHelper.ConvertDateTimeToUtc(instance.ContractStartDate, User.TimeZone());
                instance.ContractExpirationDate = ControllerHelper.ConvertDateTimeToUtc(instance.ContractExpirationDate, User.TimeZone());

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

        // GET: /Contract/Edit/5
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Contract instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
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
            ViewBag.AccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.AccountId);
            ViewBag.CompanySignedById = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.CompanySignedById);
            ViewBag.CustSignedById = new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.CustSignedById);
            ViewBag.OpportunityId = new SelectList(await _opportunityService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.OpportunityId);
            ViewBag.QuoteId = new SelectList(await _quoteService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.QuoteId);
            ViewBag.PriceBookId = new SelectList(await _priceBookService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.PriceBookId);
            ViewBag.BillingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.BillingAddressId);
            ViewBag.ShippingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ShippingAddressId);

            return View(instance);
        }

        // POST: /Contract/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,ShrtDesc,Desc,OwnerId,ContractNo,RefNo,OpportunityId,QuoteId,AccountId,PriceBookId,CustSignedById,CustSignedByTitle,CustSignedByDate,ContractStartDate,ContractTenure,ContractExpirationDate,ExpirationNotice,CompanySignedById,CompanySignedDate,DeliveryTerms,PaymentTerms,SpecialTerms,BillingAddressId,BillingAddressToName,BillingAddressLine1,BillingAddressLine2,BillingAddressLine3,BillingAddressCity,BillingAddressPin,BillingAddressState,BillingAddressCountry,BillingAddressPhone,BillingAddressEMail,ShippingAddressId,ShippingAddressToName,ShippingAddressLine1,ShippingAddressLine2,ShippingAddressLine3,ShippingAddressCity,ShippingAddressPin,ShippingAddressState,ShippingAddressCountry,ShippingAddressPhone,ShippingAddressEMail,OrgId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Authorized,TimeStamp")] Contract instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Contract model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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
                model.Desc = instance.Desc;

                model.OwnerId = instance.OwnerId;
                model.CompanySignedById = instance.CompanySignedById;
                model.CustSignedById = instance.CustSignedById.ToGuid();
                model.AccountId = instance.AccountId.ToGuid();
                model.OpportunityId = instance.OpportunityId.ToGuid();
                model.QuoteId = instance.QuoteId.ToGuid();
                model.PriceBookId = instance.PriceBookId.ToGuid();
                model.RefNo = instance.RefNo;
                model.ContractNo = instance.ContractNo;
                model.DeliveryTerms = instance.DeliveryTerms;
                model.PaymentTerms = instance.PaymentTerms;
                model.CustSignedByTitle = instance.CustSignedByTitle;
                model.CustSignedByDate = ControllerHelper.ConvertDateTimeToUtc(instance.CustSignedByDate, User.TimeZone());
                model.CompanySignedDate = ControllerHelper.ConvertDateTimeToUtc(instance.CompanySignedDate, User.TimeZone());
                model.ContractStartDate = ControllerHelper.ConvertDateTimeToUtc(instance.ContractStartDate, User.TimeZone());
                model.ContractExpirationDate = ControllerHelper.ConvertDateTimeToUtc(instance.ContractExpirationDate, User.TimeZone());
                model.ContractTenure = instance.ContractTenure;
                model.ExpirationNotice = instance.ExpirationNotice;
                model.DeliveryTerms = instance.DeliveryTerms;
                model.PaymentTerms = instance.PaymentTerms;
                model.SpecialTerms = instance.SpecialTerms;

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

        // GET: /Contract/Delete/5
        [EsAuthorize(Roles = "SalesAuthorize")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Contract model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /Contract/Delete/5
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
                    Contract model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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
                    .OrderBy(x => x.Name)
                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
        }
    }
}
