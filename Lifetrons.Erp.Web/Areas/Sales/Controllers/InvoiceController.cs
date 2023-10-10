using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Web.WebPages;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Elmah;
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
    public class InvoiceController : BaseController
    {
        private readonly IInvoiceService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IQuoteService _quoteService;
        private readonly IOpportunityService _opportunityService;
        private readonly IAccountService _accountService;
        private readonly IContactService _contactService;
        private readonly IAddressService _addressService;
        private readonly IInvoiceStatusService _invoiceStatuService;

        public InvoiceController(IInvoiceService service, IUnitOfWorkAsync unitOfWork, IOrderService orderService,
            IAspNetUserService aspNetUserService, IOpportunityService opportunityService,
            IAccountService accountService, IContactService contactService, IAddressService addressService,
            IQuoteService quoteService, IInvoiceStatusService invoiceStatuService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
            _quoteService = quoteService;
            _opportunityService = opportunityService;
            _accountService = accountService;
            _contactService = contactService;
            _addressService = addressService;
            _orderService = orderService;
            _invoiceStatuService = invoiceStatuService;
        }

        // GET: /Invoice/
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.ModifiedDate).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: /Invoice/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //ProductType instance = await _service.FindAsync(id);
            Invoice instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            instance.AspNetUser2 = ControllerHelper.GetAspNetUser(instance.OwnerId); //Owner

            instance.Account =
                await
                    _accountService.FindAsync(instance.AccountId.ToString(), applicationUser.Id,
                        applicationUser.OrgId.ToString());
            instance.Contact =
                await
                    _contactService.FindAsync(instance.ContactId.ToString(), applicationUser.Id,
                        applicationUser.OrgId.ToString());
            instance.Address =
                await
                    _addressService.FindAsync(instance.BillingAddressId.ToString(), applicationUser.Id,
                        applicationUser.OrgId.ToString());
            instance.Address1 =
                await
                    _addressService.FindAsync(instance.ShippingAddressId.ToString(), applicationUser.Id,
                        applicationUser.OrgId.ToString());
            instance.Opportunity =
                await
                    _opportunityService.FindAsync(instance.OpportunityId.ToString(), applicationUser.Id,
                        applicationUser.OrgId.ToString());
            instance.Quote =
                await
                    _quoteService.FindAsync(instance.QuoteId.ToString(), applicationUser.Id,
                        applicationUser.OrgId.ToString());
            instance.InvoiceStatu =
                await
                    _invoiceStatuService.FindAsync(instance.InvoiceStatusId.ToString(), applicationUser.Id,
                        applicationUser.OrgId.ToString());
            instance.Order =
                await
                    _orderService.FindAsync(instance.OrderId.ToString(), applicationUser.Id,
                        applicationUser.OrgId.ToString());

            ViewBag.InvoiceId = instance.Id;
            ViewBag.InvoiceName = instance.Name;

            return View(instance);
        }

        // GET: /Invoice/Create
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id",
                "Name", applicationUser.Id);

            ViewBag.AccountId =
                new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()),
                    "Id", "Name");
            ViewBag.ContactId =
                new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()),
                    "Id", "Name");
            ViewBag.OpportunityId =
                new SelectList(
                    await _opportunityService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name");
            ViewBag.QuoteId =
                new SelectList(await _quoteService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name");
            ViewBag.BillingAddressId =
                new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()),
                    "Id", "Name");
            ViewBag.ShippingAddressId =
                new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()),
                    "Id", "Name");
            ViewBag.InvoiceStatusId =
                new SelectList(
                    await _invoiceStatuService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name");
            ViewBag.OrderId =
                new SelectList(await _orderService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name");

            return View();
        }

        // POST: /Invoice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create(
            [Bind(
                Include =
                    "Name,Code,ShrtDesc,Remark,RefNo,OwnerId,AccountId,ContactId,OpportunityId,QuoteId,OrderId,InvoiceStatusId,InvoiceDate,PaymentReceiptDetails,TaxPercent,OtherCharges,BillingAddressId,BillingAddressToName,BillingAddressLine1,BillingAddressLine2,BillingAddressLine3,BillingAddressCity,BillingAddressPin,BillingAddressState,BillingAddressCountry,BillingAddressPhone,ShippingAddressId,ShippingAddressToName,ShippingAddressLine1,ShippingAddressLine2,ShippingAddressLine3,ShippingAddressCity,ShippingAddressPin,ShippingAddressState,ShippingAddressCountry,ShippingAddressPhone,PaymentTerms,Authorized"
                )] Invoice instance)
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
                instance.AccountId = instance.AccountId.ToGuid();
                instance.ContactId = instance.ContactId.ToGuid();
                instance.QuoteId = instance.QuoteId.ToGuid();
                instance.OpportunityId = instance.OpportunityId.ToGuid();
                instance.BillingAddressId = instance.BillingAddressId.ToGuid();
                instance.ShippingAddressId = instance.ShippingAddressId.ToGuid();
                instance.InvoiceStatusId = instance.InvoiceStatusId.ToGuid();
                instance.OrderId = instance.OrderId.ToGuid();
                instance.InvoiceDate = ControllerHelper.ConvertDateTimeToUtc(instance.InvoiceDate, User.TimeZone());

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

        // GET: /Invoice/Edit/5
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Invoice instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());

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

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id",
                "Name", instance.OwnerId);
            ViewBag.AccountId =
                new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()),
                    "Id", "Name", instance.AccountId);
            ViewBag.ContactId =
                new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()),
                    "Id", "Name", instance.ContactId);
            ViewBag.OpportunityId =
                new SelectList(
                    await _opportunityService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.OpportunityId);
            ViewBag.QuoteId =
                new SelectList(await _quoteService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.QuoteId);
            ViewBag.BillingAddressId =
                new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()),
                    "Id", "Name", instance.BillingAddressId);
            ViewBag.ShippingAddressId =
                new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()),
                    "Id", "Name", instance.ShippingAddressId);
            ViewBag.InvoiceStatusId =
                new SelectList(
                    await _invoiceStatuService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.InvoiceStatusId);
            ViewBag.OrderId =
                new SelectList(await _orderService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id",
                    "Name", instance.OrderId);

            return View(instance);
        }

        // POST: /Invoice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit(
            [Bind(
                Include =
                    "Id,Name,Code,ShrtDesc,Remark,RefNo,OwnerId,AccountId,ContactId,OpportunityId,QuoteId,OrderId,InvoiceStatusId,InvoiceDate,PaymentReceiptDetails,LineItemsAmount,TaxPercent,PostTaxAmount,OtherCharges,InvoiceAmount,BillingAddressId,BillingAddressToName,BillingAddressLine1,BillingAddressLine2,BillingAddressLine3,BillingAddressCity,BillingAddressPin,BillingAddressState,BillingAddressCountry,BillingAddressPhone,ShippingAddressId,ShippingAddressToName,ShippingAddressLine1,ShippingAddressLine2,ShippingAddressLine3,ShippingAddressCity,ShippingAddressPin,ShippingAddressState,ShippingAddressCountry,ShippingAddressPhone,PaymentTerms,Authorized,TimeStamp"
                )] Invoice instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Invoice model =
                    await
                        _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

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
                model.AccountId = instance.AccountId.ToGuid();
                model.ContactId = instance.ContactId.ToGuid();
                model.OpportunityId = instance.OpportunityId.ToGuid();
                model.QuoteId = instance.QuoteId;
                model.OrderId = instance.OrderId.ToGuid();
                model.RefNo = instance.RefNo;
                model.InvoiceStatusId = instance.InvoiceStatusId;
                model.InvoiceDate = ControllerHelper.ConvertDateTimeToUtc(instance.InvoiceDate, User.TimeZone());
                model.PaymentReceiptDetails = instance.PaymentReceiptDetails;
                model.TaxPercent = instance.TaxPercent;
                model.OtherCharges = instance.OtherCharges;
                model.PaymentTerms = instance.PaymentTerms;

                model.BillingAddressId = instance.BillingAddressId;
                model.BillingAddressToName = instance.BillingAddressToName;
                model.BillingAddressLine1 = instance.BillingAddressLine1;
                model.BillingAddressLine2 = instance.BillingAddressLine2;
                model.BillingAddressLine3 = instance.BillingAddressLine3;
                model.BillingAddressCity = instance.BillingAddressCity;
                model.BillingAddressPin = instance.BillingAddressPin;
                model.BillingAddressState = instance.BillingAddressState;
                model.BillingAddressCountry = instance.BillingAddressCountry;
                model.BillingAddressPhone = instance.BillingAddressPhone;

                model.ShippingAddressId = instance.ShippingAddressId;
                model.ShippingAddressToName = instance.ShippingAddressToName;
                model.ShippingAddressLine1 = instance.ShippingAddressLine1;
                model.ShippingAddressLine2 = instance.ShippingAddressLine2;
                model.ShippingAddressLine3 = instance.ShippingAddressLine3;
                model.ShippingAddressCity = instance.ShippingAddressCity;
                model.ShippingAddressPin = instance.ShippingAddressPin;
                model.ShippingAddressState = instance.ShippingAddressState;
                model.ShippingAddressCountry = instance.ShippingAddressCountry;
                model.ShippingAddressPhone = instance.ShippingAddressPhone;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        // Object Comparison and logging in audit
                        var membersToCompare = new List<string>() { "Name", "Code", "ShrtDesc", "Remark", "RefNo", "OwnerId", "AccountId", "ContactId", "OpportunityId", "QuoteId", "OrderId", "InvoiceStatusId", "InvoiceDate", "PaymentReceiptDetails", "LineItemsAmount", "TaxPercent", "PostTaxAmount", "OtherCharges", "InvoiceAmount", "BillingAddressId", "BillingAddressToName", "BillingAddressLine1", "BillingAddressLine2", "BillingAddressLine3", "BillingAddressCity", "BillingAddressPin", "BillingAddressState", "BillingAddressCountry", "BillingAddressPhone", "ShippingAddressId", "ShippingAddressToName", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingAddressLine3", "ShippingAddressCity", "ShippingAddressPin", "ShippingAddressState", "ShippingAddressCountry", "ShippingAddressPhone", "PaymentTerms", "Authorized" };
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

        // GET: /Invoice/Delete/5
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Invoice model =
                await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /Invoice/Delete/5
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
                    Invoice model =
                        await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    model.ObjectState = ObjectState.Modified;
                    model.ModifiedBy = applicationUser.Id;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Active = false;

                    _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                    //_service.Delete(id.ToString());
                    await _unitOfWork.SaveChangesAsync();

                    // Logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Deleted: InvoiceId=" + id + " InvoiceNo=" + model.InvoiceNo + " Name=" + model.Name + " Code=" + model.Code);
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
                Phone = address.Mobile + (string.IsNullOrEmpty(address.Phone1) ? "" : ", " + address.Phone1),
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

            var records = await _service.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage =
                 records.Where(x => x.Name.ToLower().Contains(searchParam.ToLower()) || x.Code.ToLower().Contains(searchParam.ToLower()))
                    .OrderByDescending(x => x.ModifiedDate)
                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
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
