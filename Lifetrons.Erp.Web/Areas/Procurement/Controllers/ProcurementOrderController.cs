using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
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
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using WebGrease.Css.Extensions;
using Task = System.Threading.Tasks.Task;

namespace Lifetrons.Erp.Procurement.Controllers
{
    [EsAuthorize(Roles = "ProcurementAuthorize, ProcurementEdit, ProcurementView")]
    public class ProcurementOrderController : BaseController
    {
        [Dependency]
        public IAccountService AccountService { get; set; }

        [Dependency]
        public IContactService ContactService { get; set; }

        [Dependency]
        public IAddressService AddressService { get; set; }

        [Dependency]
        public IProcurementOrderDetailService ProcurementOrderDetailService { get; set; }

        private readonly IProcurementOrderService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        public ProcurementOrderController(IProcurementOrderService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
        }

        // GET: /JobIssue/
        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records =  await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderByDescending(x => x.ModifiedDate).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        // GET: /JobIssue/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ProcurementOrder instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified
            
            instance.Account = await AccountService.FindAsync(instance.AccountId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Contact = await ContactService.FindAsync(instance.ContactId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Address = await AddressService.FindAsync(instance.SupplierAddressId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Address1 = await AddressService.FindAsync(instance.BillingAddressId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Address2 = await AddressService.FindAsync(instance.ShippingAddressId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            instance.ProcurementOrderDetails = (await ProcurementOrderDetailService.SelectAsyncLineItems(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString())).ToList();
            
            //Set id to pass on detail partial page
            ViewBag.ProcurementOrderId = instance.Id;

            return View(instance);
        }

        // GET: /JobIssue/Create
        [EsAuthorize(Roles = "ProcurementAuthorize, ProcurementEdit")]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

           ViewBag.AccountId = new SelectList(await AccountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
           ViewBag.ContactId = new SelectList(await ContactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
           ViewBag.SupplierAddressId = new SelectList(await AddressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
           ViewBag.BillingAddressId = new SelectList(await AddressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
           ViewBag.ShippingAddressId = new SelectList(await AddressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        // POST: /JobIssue/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ProcurementAuthorize, ProcurementEdit")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Code,RefNo, Date,AccountId,ContactId,DeliveryDate,DeliveryTerms,PaymentTerms,SpecialTerms,Remark,ReceiptDetails,TaxPercent,OtherCharges," +
                                                               "SupplierAddressId,SupplierAddressToName,SupplierAddressLine1,SupplierAddressLine2,SupplierAddressLine3,SupplierAddressCity,SupplierAddressPin,SupplierAddressState,SupplierAddressCountry,SupplierAddressPhone,SupplierAddressEMail," +
                                                               "BillingAddressId,BillingAddressToName,BillingAddressLine1,BillingAddressLine2,BillingAddressLine3,BillingAddressCity,BillingAddressPin,BillingAddressState,BillingAddressCountry,BillingAddressPhone,BillingAddressEMail," +
                                                               "ShippingAddressId,ShippingAddressToName,ShippingAddressLine1,ShippingAddressLine2,ShippingAddressLine3,ShippingAddressCity,ShippingAddressPin,ShippingAddressState,ShippingAddressCountry,ShippingAddressPhone,ShippingAddressEMail" +
                                                               "Authorized,TimeStamp")] ProcurementOrder instance)
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
                instance.Name = instance.Name;
                instance.Code = instance.Name + Helper.SysSeparator + DateTime.UtcNow;
                
                instance.AccountId = instance.AccountId.ToGuid();
                instance.ContactId = instance.ContactId.ToGuid();
                instance.SupplierAddressId = instance.SupplierAddressId.ToGuid();
                instance.BillingAddressId = instance.BillingAddressId.ToGuid();
                instance.ShippingAddressId = instance.ShippingAddressId.ToGuid();

                instance.Date = ControllerHelper.ConvertDateTimeToUtc(instance.Date, User.TimeZone());
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

        // GET: /JobIssue/Edit/5
        [EsAuthorize(Roles = "ProcurementAuthorize, ProcurementEdit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ProcurementOrder instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            //If instance.Authorized, only user with "ProcurementAuthorize" role is allowed to edit the record
            if (instance.Authorized && !User.IsInRole("ProcurementAuthorize"))
            {
                var exception = new System.ApplicationException("Only authorized user can edit a record marked 'Authorized'.");
                TempData["CustomErrorMessage"] = "Only authorized user can edit a record marked 'Authorized'.";
                throw exception;
            }

            await SetupInstanceForm(applicationUser, instance);

            return View(instance);
        }

        private async Task SetupInstanceForm(ApplicationUser applicationUser, ProcurementOrder instance)
        {
            ViewBag.AccountId = new SelectList(await AccountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.AccountId);
            ViewBag.ContactId = new SelectList(await ContactService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ContactId);
            ViewBag.SupplierAddressId = new SelectList(await AddressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.SupplierAddressId);
            ViewBag.BillingAddressId = new SelectList(await AddressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.BillingAddressId);
            ViewBag.ShippingAddressId = new SelectList(await AddressService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.ShippingAddressId);
        }

        // POST: /JobIssue/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ProcurementAuthorize, ProcurementEdit")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,RefNo, Date,AccountId,ContactId,DeliveryDate,DeliveryTerms,PaymentTerms,SpecialTerms,Remark,ReceiptDetails,TaxPercent,OtherCharges," +
                                                               "SupplierAddressId,SupplierAddressToName,SupplierAddressLine1,SupplierAddressLine2,SupplierAddressLine3,SupplierAddressCity,SupplierAddressPin,SupplierAddressState,SupplierAddressCountry,SupplierAddressPhone,SupplierAddressEMail," +
                                                               "BillingAddressId,BillingAddressToName,BillingAddressLine1,BillingAddressLine2,BillingAddressLine3,BillingAddressCity,BillingAddressPin,BillingAddressState,BillingAddressCountry,BillingAddressPhone,BillingAddressEMail," +
                                                               "ShippingAddressId,ShippingAddressToName,ShippingAddressLine1,ShippingAddressLine2,ShippingAddressLine3,ShippingAddressCity,ShippingAddressPin,ShippingAddressState,ShippingAddressCountry,ShippingAddressPhone,ShippingAddressEMail" +
                                                               "Authorized,TimeStamp")] ProcurementOrder instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                ProcurementOrder model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                //If user is in role of  ProcurementAuthorize role, process editing. Also update status in long desc field.
                if (User.IsInRole("ProcurementAuthorize"))
                {
                    if (model.Authorized != instance.Authorized)
                    {
                        model.Authorized = instance.Authorized;
                        instance.Remark = instance.Authorized ? "[Authorized] " + instance.Remark : "[UnAuthorized] " + instance.Remark;
                    }
                }
                //If user is not in role of ProcurementAuthorize and record is Authorized, stop editng and show appropriate message.
                else if (model.Authorized)
                {
                    TempData["CustomErrorMessage"] = "Authorized record cannot be edited. Please Un-authorize the record to enable editing." +
                                                     "You should have authorization rights to authorize/unauthorize records.";
                    return RedirectToAction("Error", "Error", new { message = TempData["CustomErrorMessage"] });
                }

                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;
                model.Name = instance.Name;
                model.Code = string.IsNullOrEmpty(instance.Code)
                           ? instance.Name + Helper.SysSeparator + DateTime.UtcNow
                           : instance.Code;
                model.RefNo = instance.RefNo;

                string instanceRemark = instance.Remark.IsEmpty() ? string.Empty : applicationUser.UserName + ":" + instance.Remark;
                model.Remark = model.Remark.IsEmpty() ? instanceRemark : model.Remark + "\n" + instanceRemark;
                
                model.AccountId = instance.AccountId.ToGuid();
                model.ContactId = instance.ContactId.ToGuid();
                
                model.Date = ControllerHelper.ConvertDateTimeToUtc(instance.Date, User.TimeZone());
                model.DeliveryDate = ControllerHelper.ConvertDateTimeToUtc(instance.DeliveryDate, User.TimeZone());

                model.TaxPercent = instance.TaxPercent;
                model.OtherCharges = instance.OtherCharges;
                
                model.SpecialTerms = instance.SpecialTerms;
                model.DeliveryTerms = instance.DeliveryTerms;
                model.PaymentTerms = instance.PaymentTerms;
                model.ReceiptDetails = instance.ReceiptDetails;

                model.SupplierAddressId = instance.SupplierAddressId.ToGuid();
                model.SupplierAddressToName = instance.SupplierAddressToName;
                model.SupplierAddressLine1 = instance.SupplierAddressLine1;
                model.SupplierAddressLine2 = instance.SupplierAddressLine2;
                model.SupplierAddressLine3 = instance.SupplierAddressLine3;
                model.SupplierAddressCity = instance.SupplierAddressCity;
                model.SupplierAddressPin = instance.SupplierAddressPin;
                model.SupplierAddressState = instance.SupplierAddressState;
                model.SupplierAddressCountry = instance.SupplierAddressCountry;
                model.SupplierAddressPhone = instance.SupplierAddressPhone;
                model.SupplierAddressEMail = instance.SupplierAddressEMail;

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
                        var membersToCompare = new List<string>() { "Name", "Code", "RefNo", "EmployeeId", "DepartmentId", "Date", "Location", "Remark", "Authorized" };
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

        // GET: /JobIssue/Delete/5
        [EsAuthorize(Roles = "ProcurementAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ProcurementOrder model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }

            model.AspNetUser = ControllerHelper.GetAspNetUser(model.CreatedBy); //Created
            model.AspNetUser1 = ControllerHelper.GetAspNetUser(model.ModifiedBy); // Modified

            model.Account = await AccountService.FindAsync(model.AccountId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Contact = await ContactService.FindAsync(model.ContactId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Address = await AddressService.FindAsync(model.SupplierAddressId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Address1 = await AddressService.FindAsync(model.BillingAddressId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            model.Address2 = await AddressService.FindAsync(model.ShippingAddressId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.Id = model.Id;
            ViewBag.Name = model.Name;

            return View(model);
        }

        // POST: /JobIssue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "ProcurementAuthorize")]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    ProcurementOrder model = await _service.FindAsync(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                    var procurementOrderDetail = ProcurementOrderDetailService.SelectLineItems(id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    string logEntry = "Deleted ProcurementOrder: Id=" + id + " Name=" + model.Name + " Date=" + model.Date + " RefNo=" + model.RefNo + " Account=" + model.AccountId + " ContactId=" + model.ContactId;
                    procurementOrderDetail.ForEach(p => logEntry += "\n ProcurementOrderDetail: Id=" + p.Id + " ProcurementOrderId=" + p.ProcurementOrderId + " Status=" + p.Status + " Item=" + p.ItemId + " Quantity=" + p.Quantity);
                    
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

        public async Task<ActionResult> Search(string searchParam, int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            var records = await _service.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage =
                 records.Where(x => x.Name.ToLower().Contains(searchParam.ToLower()) || x.RefNo.ToLower().Contains(searchParam.ToLower()))
                    .OrderByDescending(x => x.ModifiedDate)
                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
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

            var address = AddressService.Find(param, applicationUser.Id, applicationUser.OrgId.ToString());

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
                Phone = address.Mobile + (string.IsNullOrEmpty(address.Phone1)? "" :  ", " + address.Phone1),
                EMail = address.AddressToEMail,
            };

            // Convert anonymous object to JSON response
            response = Json(returnData, JsonRequestBehavior.AllowGet);

            // Return JSON
            return response;
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
        }

        private string HandleDbEntityValidationException(DbEntityValidationException dbEntityValidationException)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(dbEntityValidationException));
            // Retrieve the error messages as a list of strings.
            var errorMessages = dbEntityValidationException.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

            // Join the list to a single string.
            var fullErrorMessage = string.Join("; ", errorMessages);

            // Combine the original exception message with the new one.
            var exceptionMessage = string.Concat(dbEntityValidationException.Message, " The validation errors are: ", fullErrorMessage);

            // Throw a new DbEntityValidationException with the improved exception message.
            TempData["CustomErrorMessage"] = exceptionMessage;
            TempData["CustomErrorDetail"] = dbEntityValidationException.InnerException.Message;
            AddErrors(exceptionMessage);

            return exceptionMessage;
        }

        private void HandleException(Exception exception)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
            TempData["CustomErrorMessage"] = exception.Message;
            TempData["CustomErrorDetail"] = exception.InnerException.Message;
            AddErrors(exception.Message.ToString() + " - " + exception.InnerException.Message.ToString() + " ___" + DateTime.UtcNow);
        }

        private void AddErrors(string errorMessage)
        {
            ModelState.AddModelError("", errorMessage);
        }

        #endregion Handle Exception
    }
}
