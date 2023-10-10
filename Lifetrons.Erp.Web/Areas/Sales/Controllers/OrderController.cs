using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Web.WebPages;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
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
using WebGrease.Css.Extensions;

namespace Lifetrons.Erp.Sales.Controllers
{
    [EsAuthorize(Roles = "SalesAuthorize, SalesEdit, SalesView")]
    public class OrderController : BaseController
    {
        [Dependency]
        public IOrderLineItemService OrderLineItemService { get; set; }

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

        public OrderController(IOrderService service, IUnitOfWorkAsync unitOfWork,
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
            var enumerablePage = records.OrderByDescending(x => x.CreatedDate).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

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

        // GET: /Order/Create
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.OwnerId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.ActivatedById = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);
            ViewBag.CompanySignedById = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);

            ViewBag.AccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.SubAccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.CustSignedById = new SelectList(await _contactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.OpportunityId = new SelectList(await _opportunityService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.QuoteId = new SelectList(await _quoteService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.BillingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ShippingAddressId = new SelectList(await _addressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.DeliveryStatusId = new SelectList(await _deliveryStatuService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.PriorityId = new SelectList(await _priorityService.GetAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ContractId = new SelectList(await _contractService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ReportCompletionToId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", applicationUser.Id);

            return View();
        }

        // POST: /Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Code,ShrtDesc,Remark,OwnerId,ContractId,OpportunityId,QuoteId,AccountId,SubAccountId,RefNo,PriorityId,CustSignedById,CustSignedByDate,CompanySignedById,StartDate,ActivatedDate,ActivatedById,DeliveryDate,DeliveryStatusId,ProgressPercent,ProgressDesc,ReportCompletionToId,DeliveryTerms,PaymentTerms,SpecialTerms,BillingAddressId,BillingAddressToName,BillingAddressLine1,BillingAddressLine2,BillingAddressLine3,BillingAddressCity,BillingAddressPin,BillingAddressState,BillingAddressCountry,BillingAddressPhone,BillingAddressEMail,ShippingAddressId,ShippingAddressToName,ShippingAddressLine1,ShippingAddressLine2,ShippingAddressLine3,ShippingAddressCity,ShippingAddressPin,ShippingAddressState,ShippingAddressCountry,ShippingAddressPhone,ShippingAddressEMail,Authorized")] Order instance)
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
                instance.Remark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                instance.ProgressDesc = instance.ProgressDesc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.ProgressDesc;
                instance.AccountId = instance.AccountId.ToGuid();
                instance.SubAccountId = instance.SubAccountId.ToGuid();
                instance.CustSignedById = instance.CustSignedById.ToGuid();
                instance.QuoteId = instance.QuoteId.ToGuid();
                instance.OpportunityId = instance.OpportunityId.ToGuid();
                instance.BillingAddressId = instance.BillingAddressId.ToGuid();
                instance.ShippingAddressId = instance.ShippingAddressId.ToGuid();
                instance.DeliveryStatusId = instance.DeliveryStatusId.ToGuid();
                instance.PriorityId = instance.PriorityId.ToGuid();
                instance.ActivatedDate = ControllerHelper.ConvertDateTimeToUtc(instance.ActivatedDate, User.TimeZone());
                instance.CustSignedByDate = ControllerHelper.ConvertDateTimeToUtc(instance.CustSignedByDate, User.TimeZone());
                instance.StartDate = ControllerHelper.ConvertDateTimeToUtc(instance.StartDate, User.TimeZone());
                instance.DeliveryDate = ControllerHelper.ConvertDateTimeToUtc(instance.DeliveryDate, User.TimeZone());

                try
                {
                    _service.Create(instance, applicationUser.Id, applicationUser.OrgId.ToString());
                    await _unitOfWork.SaveChangesAsync();
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

        // GET: /Order/Edit/5
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
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

            //If instance.Authorized, only user with "SalesAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("SalesAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            await SetupInstanceForm(applicationUser, instance);

            return View(instance);
        }

        private async System.Threading.Tasks.Task SetupInstanceForm(ApplicationUser applicationUser, Order instance)
        {

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

        }

        // POST: /Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "SalesAuthorize, SalesEdit")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,ShrtDesc,Remark,OwnerId,ContractId,OpportunityId,QuoteId,AccountId,SubAccountId,RefNo,PriorityId,CustSignedById,CustSignedByDate,CompanySignedById,StartDate,ActivatedDate,ActivatedById,DeliveryDate,DeliveryStatusId,ProgressPercent,ProgressDesc,ReportCompletionToId,DeliveryTerms,PaymentTerms,SpecialTerms,BillingAddressId,BillingAddressToName,BillingAddressLine1,BillingAddressLine2,BillingAddressLine3,BillingAddressCity,BillingAddressPin,BillingAddressState,BillingAddressCountry,BillingAddressPhone,BillingAddressEMail,ShippingAddressId,ShippingAddressToName,ShippingAddressLine1,ShippingAddressLine2,ShippingAddressLine3,ShippingAddressCity,ShippingAddressPin,ShippingAddressState,ShippingAddressCountry,ShippingAddressPhone,ShippingAddressEMail,Authorized,TimeStamp")] Order instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                Order model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  SalesAuthorize role, process editing. Also update status in long desc field.
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
                model.ActivatedById = instance.ActivatedById;
                model.ActivatedDate = ControllerHelper.ConvertDateTimeToUtc(instance.ActivatedDate, User.TimeZone());
                model.CompanySignedById = instance.CompanySignedById;
                model.ContractId = instance.ContractId.ToGuid();
                model.CustSignedById = instance.CustSignedById.ToGuid();
                model.CustSignedByDate = ControllerHelper.ConvertDateTimeToUtc(instance.CustSignedByDate, User.TimeZone());
                model.AccountId = instance.AccountId.ToGuid();
                model.SubAccountId = instance.SubAccountId.ToGuid();
                model.OpportunityId = instance.OpportunityId.ToGuid();
                model.QuoteId = instance.QuoteId;
                model.RefNo = instance.RefNo;
                model.DeliveryTerms = instance.DeliveryTerms;
                model.PaymentTerms = instance.PaymentTerms;
                model.SpecialTerms = instance.SpecialTerms;
                model.PriorityId = instance.PriorityId;
                model.StartDate = ControllerHelper.ConvertDateTimeToUtc(instance.StartDate, User.TimeZone());
                model.DeliveryDate = ControllerHelper.ConvertDateTimeToUtc(instance.DeliveryDate, User.TimeZone());
                model.DeliveryStatusId = instance.DeliveryStatusId;
                model.ProgressPercent = instance.ProgressPercent;
                string instanceProgressDesc = instance.ProgressDesc.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.ProgressDesc;
                model.ProgressDesc = model.ProgressDesc.IsEmpty() ? instanceProgressDesc : model.ProgressDesc + "\n" + instanceProgressDesc;
                model.ReportCompletionToId = instance.ReportCompletionToId;

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
                        var membersToCompare = new List<string>() { "Name", "Code", "OrderNo", "ShrtDesc", "Remark", "OwnerId", "ContractId", "OpportunityId", "QuoteId", "AccountId", "SubAccountId", "RefNo", "PriorityId", "CustSignedById", "CustSignedByDate", "CompanySignedById", "StartDate", "ActivatedDate", "ActivatedById", "DeliveryDate", "DeliveryStatusId", "ProgressPercent", "ProgressDesc", "ReportCompletionToId", "DeliveryTerms", "PaymentTerms", "SpecialTerms", "BillingAddressId", "BillingAddressToName", "BillingAddressLine1", "BillingAddressLine2", "BillingAddressLine3", "BillingAddressCity", "BillingAddressPin", "BillingAddressState", "BillingAddressCountry", "BillingAddressPhone", "BillingAddressEMail", "ShippingAddressId", "ShippingAddressToName", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingAddressLine3", "ShippingAddressCity", "ShippingAddressPin", "ShippingAddressState", "ShippingAddressCountry", "ShippingAddressPhone", "ShippingAddressEMail", "Authorized" };
                        var compareResult = new ControllerHelper().Compare(model, instance, membersToCompare);
                        if (!compareResult.AreEqual) HttpContext.Items.Add(ControllerHelper.AuditData, compareResult.DifferencesString);

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


        // GET: /Order/Delete/5
        [EsAuthorize(Roles = "SalesAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            Order model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /Order/Delete/5
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
                    Order model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    var lineItems = OrderLineItemService.SelectLineItems(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    string logEntry = "Deleted OrderId: Id=" + id + "OrderNo =" + model.OrderNo +  " AccountId = " + model.AccountId + " SubAccountId = " + model.SubAccountId + " Start Date=" + model.StartDate + " OwnerId=" + model.OwnerId + " RefNo=" + model.RefNo;
                    lineItems.ForEach(p => logEntry += "\n LineItems: Id=" + p.Id + " OrderId=" + p.OrderId + " JobNo=" + p.JobNo + " ProductId=" + p.PriceBookId + " PriceBookId=" + p.PriceBookId + " Quantity=" + p.Quantity);

                    _service.Delete(model);
                    await _unitOfWork.SaveChangesAsync();

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

            //var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
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
                 records.Where(x => x.Name.ToLower().Contains(searchParam.ToLower()) || 
                     x.Code.ToLower().Contains(searchParam.ToLower()) || 
                     x.OrderNo.ToString().Contains(searchParam.ToLower()) || 
                     x.RefNo.ToLower().Contains(searchParam.ToLower()))
                    .OrderByDescending(x => x.ModifiedDate)
                    .ToPagedList(pageNumber, 50); // will only contain 25 products max because of the pageSize

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
                    Name = "TO " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
                    Code = "TO " + name + Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, applicationUser.TimeZone),
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
            else if (innerExceptionMessage.Contains("JobNo"))
            {
                AddErrors("Invalid Job No.");
                TempData["CustomErrorMessage"] = "Invalid Job No.";

            }
            else if (innerExceptionMessage.Contains("CK_ProdPlan_CheckProdPlanProcess"))
            {
                AddErrors("Process cannot be changed for this plan. Already scheduled jobs.");
                TempData["CustomErrorMessage"] = "Invalid Job No.";
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
