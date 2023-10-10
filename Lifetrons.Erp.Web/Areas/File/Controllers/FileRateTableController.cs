using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.WebPages;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
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
using System.IO;
using Lifetrons.Erp.Web.Models.Repository;
using Lifetrons.Erp.Web.Domain.Twilio.Notifications;
using System.Threading;
using System.Web.Hosting;
using Twilio.Rest.Api.V2010.Account;

namespace Lifetrons.Erp.File.Controllers
{
    [EsAuthorize(Roles = "FileAuthorize, FileUpload, FileDownload")]
    public class FileRateTableController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IContactService _contactService;
        private readonly IFileRateTableService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        public FileRateTableController(IFileRateTableService service, IUnitOfWorkAsync unitOfWork, IAccountService accountService, IContactService contactService, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _accountService = accountService;
            _contactService = contactService;
            _aspNetUserService = aspNetUserService;
        }

        public async Task<ActionResult> Index(int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var records = await _service.GetAsync(applicationUser.OrgId.ToString());
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage = records.OrderBy(x => x.Contact.Name).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View(enumerablePage);
        }

        [EsAuthorize(Roles = "FileAdmin")]
        [HttpGet]
        public async Task<ActionResult> ManageUpload(string fName)
        {
            var file = _service.Find(fName);
            TempData["id"] = fName;
            return View("Upload");
        }

        [EsAuthorize(Roles = "FileAdmin")]
        [Audit(AuditingLevel = 0)]
        [HttpPost]
        public async Task<ActionResult> UploadFile(string id)
        {
            //Get application user for verification
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            foreach (string upload in Request.Files)
            {
                if (Request.Files[upload].FileName != "")
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/Templates/";
                    string filename = System.IO.Path.GetFileName(Request.Files[upload].FileName);
                    string fileExtension = filename.Substring(filename.IndexOf("."));
                    filename = filename.Insert(filename.IndexOf("."), Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, User.TimeZone()));
                    filename = filename.Replace(":", "-");

                    Request.Files[upload].SaveAs(System.IO.Path.Combine(path, filename));

                    //Convert Request.Files to byte array
                    byte[] fileData = null;
                    fileData = Data.Helper.StreamToByte(Request.Files[upload].InputStream);

                    //Save
                    var template = _service.Find(id);
                    template.TemplateUrl = "/App_Data/Templates/" + filename;
                    template.TemplateUpdateDate = DateTime.UtcNow;

                    _service.Update(template, applicationUser.Id, applicationUser.OrgId.ToString());
                    _unitOfWork.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        [EsAuthorize(Roles = "FileDownload")]
        public FileResult DownloadTemplate(string id)
        {
            var file = _service.Find(id);
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //var FileVirtualPath = "~/App_Data/uploads/" + file.FileName;
            var FileVirtualPath = "~" + file.TemplateUrl;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));

        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var instance = await _service.FindAsync(id);

            if (instance == null)
            {
                return HttpNotFound();
            }

            instance.Account = await _accountService.FindAsync(instance.AccountId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            // instance.Account1 = await _accountService.FindAsync(instance.SubAccountId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
            instance.Contact = await _contactService.FindAsync(instance.ContactId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            instance.AspNetUser = ControllerHelper.GetAspNetUser(instance.CreatedBy); //Created
            instance.AspNetUser1 = ControllerHelper.GetAspNetUser(instance.ModifiedBy); // Modified

            ViewBag.FileRateTableId = instance.Id;

            return View(instance);
        }

        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            ViewBag.AccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.SubAccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            ViewBag.ContactId = new SelectList(await _contactService.SelectAsyncAccountNotNull(applicationUser.OrgId.ToString()), "Id", "Name");

            return View();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FileType,TemplateName,ContactId,AccountId,SubAccountId,RateType,Rate,StartDate,EndDate,Active,Authorized,Desc")] FileRateTable instance)
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
                instance.Authorized = true;
                instance.Active = true;
                instance.TemplateUrl = "/App_Data/Templates/sample.pdf";
                instance.TemplateUpdateDate = DateTime.UtcNow;
                instance.StartDate = ControllerHelper.ConvertDateTimeToUtc(instance.StartDate, User.TimeZone());
                instance.EndDate = ControllerHelper.ConvertDateTimeToUtc(instance.EndDate, User.TimeZone());

                //Fill up account from contact only
                var contact = _contactService.Find(instance.ContactId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                instance.AccountId = contact.AccountId.ToSysGuid();

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

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var instance = await _service.FindAsync(id);
            if (instance == null)
            {
                return HttpNotFound();
            }

            ViewBag.AccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.AccountId);
            ViewBag.SubAccountId = new SelectList(await _accountService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name", instance.SubAccountId);
            ViewBag.ContactId = new SelectList(await _contactService.SelectAsyncAccountNotNull(applicationUser.OrgId.ToString()), "Id", "Name", instance.ContactId);

            return View(instance);
        }

        // POST: 
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FileType,TemplateName,ContactId,AccountId,SubAccountId,RateType,Rate,StartDate,EndDate,Active,Authorized,Desc")] FileRateTable instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                var model = await _service.FindAsync(instance.Id.ToString());
                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;
                model.FileType = instance.FileType;
                model.TemplateName = instance.TemplateName;

                model.Rate = instance.Rate;
                model.RateType = instance.RateType;
                model.StartDate = ControllerHelper.ConvertDateTimeToUtc(instance.StartDate, User.TimeZone());
                model.EndDate = ControllerHelper.ConvertDateTimeToUtc(instance.EndDate, User.TimeZone());
                model.Desc = instance.Desc;
                model.ContactId = instance.ContactId;

                //Fill up account from contact only
                var contact = _contactService.Find(instance.ContactId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());
                model.AccountId = contact.AccountId.ToSysGuid();
                //model.SubAccountId = instance.SubAccountId;                
                model.Authorized = instance.Authorized;
                model.Active = true;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        _unitOfWork.SaveChanges();

                        ////Send message
                        var message = string.Format("Template/Rate Updated : " + model.TemplateName);
                        var notifier = new Notifier(new AdministratorsRepository());
                        var smsResult = await notifier.SendMessagesAsync(message);

                        foreach (var item in smsResult)
                        {
                            if(item.Status == MessageResource.StatusEnum.Failed)
                            {
                                ViewBag.SMSMessage = item.To + " " + item.ErrorMessage;
                            }
                        }

                        //Action<CancellationToken> workItem = PostToRemoteService;
                        //HostingEnvironment.QueueBackgroundWorkItem(workItem);


                        //System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                        //{
                        //    var message = string.Format("Template/Rate Updated : " + model.TemplateName);
                        //    var notifier = new Notifier(new AdministratorsRepository());
                        //    //// For small running job
                        //    // notifier.SendMessagesAsync(message);

                        //    // For long running job. But use "async cancellationToken =>" instead of "cancellationToken =>"
                        //    //var result = await LongRunningMethodAsync();
                        //    // Do something with result

                        //});
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

        // GET: 
        [EsAuthorize(Roles = "FileAuthorize")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var model = await _service.FindAsync(id.ToString());
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [EsAuthorize(Roles = "FileAuthorize")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                if (applicationUser.OrgId != null) //Only approver is allowed
                {
                    var model = await _service.FindAsync(id.ToString());

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

        public async Task<ActionResult> Search(string searchParam, int? page)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var records = await _service.GetAsync(applicationUser.OrgId.ToString());

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var enumerablePage =
                 records.Where(x => x.TemplateName.ToLower().Contains(searchParam.ToLower()) || x.Contact.Name.ToLower().Contains(searchParam.ToLower()))
                    .OrderByDescending(x => x.ModifiedDate)
                    .ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

            return View("Index", enumerablePage);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        private void QueueWorkItem()
        {
            Func<CancellationToken, System.Threading.Tasks.Task> workItem = LongRunningMethodAsync;
            System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(workItem);
        }

        private async System.Threading.Tasks.Task LongRunningMethodAsync(CancellationToken cancellationToken)
        {
            // Some long-running job
            var message = string.Format("Template/Rate Updated");
            var notifier = new Notifier(new AdministratorsRepository());
            notifier.SendMessagesAsync(message);
        }

        private async void PostToRemoteService(CancellationToken cancellationToken)
        {
            //using (var client = new HttpClient())
            //{
            //    var response = await client.PostAsync("http://example.com/endpoint",
            //        new StringContent("..."), cancellationToken);

            //    // Do something with response
            //    // ...
            //}
            // Some long-running job
            var message = string.Format("Template/Rate Updated");
            var notifier = new Notifier(new AdministratorsRepository());
            notifier.SendMessagesAsync(message);
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
