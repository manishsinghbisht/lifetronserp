using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using OpenPop.Mime;
using OpenPop.Pop3;
using PagedList;
using Repository;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Word;
using DocumentFormat.OpenXml;
using System.Timers;
using Microsoft.Practices.Unity;
using HtmlAgilityPack;
using Lifetrons.Erp.Web.Domain.Twilio.Notifications;
using Twilio.Rest.Api.V2010.Account;
using Lifetrons.Erp.Web.Models.Repository;

namespace Lifetrons.Erp.File.Controllers
{
    [EsAuthorize(Roles = "FileAuthorize, FileUpload, FileDownload, FileAdmin")]
    public class FileController : BaseController
    {
        private readonly IFileService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;
        private readonly IAccountService _accountService;
        private readonly IContactService _contactService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly ITaskService _taskService;
        private readonly IFileRateTableService _fileRateTableService;
        private readonly INoticeBoardService _noticeBoardService;
        private static readonly Timer aTimer = new System.Timers.Timer(50000);
        private static DateTime DtDownloadEmailsJob;
        private static bool IsDownloadEmailsJobRunning = false;
        private int JobsCreatedByDownloadEmailsRunJob;

        public FileController(IFileService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService, IFileRateTableService fileRateTableService,
            IAccountService accountService, IContactService contactService, ITaskService taskService, IEmailConfigService emailConfigService, INoticeBoardService noticeBoardService)
        {
            _service = service;
            _fileRateTableService = fileRateTableService;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
            _accountService = accountService;
            _contactService = contactService;
            _taskService = taskService;
            _emailConfigService = emailConfigService;
            _noticeBoardService = noticeBoardService;

            //aTimer.Elapsed -= new ElapsedEventHandler(RunThis);
            //aTimer.Elapsed += new ElapsedEventHandler(RunThis);
            //aTimer.AutoReset = true;
            //aTimer.Enabled = true;
        }

        public async Task<ActionResult> Index()
        {
            //Get application user for verification
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            //Load basic user info
            TempData["Username"] = applicationUser.UserName;
            TempData["UserId"] = applicationUser.Id;

            //Check for new Notices accordian exapands according to OpeningDate
            TempData["NewNoticeExists"] = false;
            var noticeBoard = _noticeBoardService.Get(applicationUser.Id, applicationUser.OrgId.ToString());
            var firstordefaultNotice = noticeBoard.FirstOrDefault();
            if (firstordefaultNotice != null)
            {
                noticeBoard.ForEach(p =>
                {
                    if (p.OpeningDate.Date == DateTime.Today.Date && p.OpeningDate.Date <= DateTime.Today.Date.AddDays(2))
                    {
                        TempData["NewNoticeExists"] = true;
                    }
                });
            }
            TempData["noticeBoard"] = noticeBoard;

            var assignedFiles = _service.FindByUserAndStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Assigned.ToString(), applicationUser.Id);
            TempData["Files"] = assignedFiles.ToList();

            var submittedFilesEnumerable = _service.FindByUserAndStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Submitted.ToString(), applicationUser.Id);
            var deliveredFilesEnumerable = _service.FindByUserAndStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Delivered.ToString(), applicationUser.Id);

            var submittedFiles = submittedFilesEnumerable.ToList();
            var deliveredFiles = deliveredFilesEnumerable.ToList();
            
            TempData["SubmittedFiles"] = submittedFiles.ToList();
            TempData["DeliveredFiles"] = deliveredFiles.ToList();

            ////Download emails job
            RunJob();

            return View();
        }

        public async Task<ActionResult> JobsSummary()
        {
            //Get application user for verification
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var queuedFiles = _service.FindByStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Queued.ToString(), applicationUser.OrgId.ToString());
            TempData["QueuedFiles"] = queuedFiles.ToList();

            var reviewFiles = _service.FindByStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Review.ToString(), applicationUser.OrgId.ToString());
            TempData["ReviewFiles"] = reviewFiles.ToList();

            var assignedFiles = _service.FindByStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Assigned.ToString(), applicationUser.OrgId.ToString());
            TempData["AssignedFiles"] = assignedFiles.ToList();

            var submittedFiles = _service.FindByStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Submitted.ToString(), applicationUser.OrgId.ToString());
            TempData["submittedFiles"] = submittedFiles.ToList();

            var deliveredFiles = _service.FindByStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Delivered.ToString(), applicationUser.OrgId.ToString());
            TempData["DeliveredFiles"] = deliveredFiles.ToList();

            var rejectedFiles = _service.FindByStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Rejected.ToString(), applicationUser.OrgId.ToString());
            TempData["RejectedFiles"] = rejectedFiles.ToList();

            return View();
        }
        public async Task<ActionResult> JobsInQueue()
        {
            //Get application user for verification
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            //Load basic user info
            TempData["Username"] = applicationUser.UserName;
            TempData["UserId"] = applicationUser.Id;

            var queuedFiles = _service.FindByStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Queued.ToString(), applicationUser.OrgId.ToString());
            TempData["QueuedFiles"] = queuedFiles.ToList();

            var reviewFiles = _service.FindByStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Review.ToString(), applicationUser.OrgId.ToString());
            TempData["ReviewFiles"] = reviewFiles.ToList();

            var assignedFiles = _service.FindByStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Assigned.ToString(), applicationUser.OrgId.ToString());
            TempData["AssignedFiles"] = assignedFiles.ToList();

            var submittedFiles = _service.FindByStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Submitted.ToString(), applicationUser.OrgId.ToString());
            TempData["submittedFiles"] = submittedFiles.ToList();

            var deliveredFiles = _service.FindByStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Delivered.ToString(), applicationUser.OrgId.ToString());
            TempData["DeliveredFiles"] = deliveredFiles.ToList();

            var rejectedFiles = _service.FindByStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Rejected.ToString(), applicationUser.OrgId.ToString());
            TempData["RejectedFiles"] = rejectedFiles.ToList();

            return View();
        }

        public async Task<ActionResult> RunDownloadJob()
        {
            //Download emails job
            RunJob();
            TempData["MessageResult"] = "Files created: " + JobsCreatedByDownloadEmailsRunJob.ToString();
            // return Content("Files created: " + JobsCreatedByDownloadEmailsRunJob.ToString());
            return View("MessageResult");
        }

        public async Task<ActionResult> AssignFiles()
        {
            string assignmentMessage = "";
            var files = AssignFile(out assignmentMessage);
            TempData["assignmentMessage"] = assignmentMessage;

            TempData["MessageResult"] = "Assignment detail: " + assignmentMessage;
            return View("MessageResult");
        }

        private List<Data.File> AssignFile(out string message, string userId = "")
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(userId))
            {
                userId = applicationUser.Id;
            }

            message = "";
            List<Data.File> files = null;
            if (_service.AssignFileToUser(userId, out message))
            {
                var assignedFiles = _service.FindByUserAndStatus(Helper.FileType.CV.ToString(), Helper.FileStatus.Assigned.ToString(), userId);
                if (assignedFiles != null)
                {
                    if (assignedFiles.Count() >= 1)
                    {
                        files = assignedFiles.ToList();
                    }
                }
            }

            return files;
        }

        public async Task<ActionResult> ChangeStatusToReview(string id)
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            
            var file = await _service.FindAsync(id);
            try
            {
                file.ModifiedDate = DateTime.UtcNow;
                file.ModifiedBy = applicationUser.Id;
                file.Status = Data.Helper.FileStatus.Review.ToString();
                _service.Update(file, applicationUser.Id, applicationUser.OrgId.ToString());
                _unitOfWork.SaveChanges();

                TempData["MessageResult"] = "File successfully sent for review!";
                return View("MessageResult");
            }
            catch (Exception ex)
            {

                throw new Exception("Error in changing status fo file to Review." + ex.Message);
            }
            
            return View("MessageResult");
        }

        [EsAuthorize(Roles = "FileUpload")]
        [HttpGet]
        public async Task<ActionResult> ManageUpload(string fName, string uName)
        {
            //fName= fileId, uName = userId
            var user = _aspNetUserService.Find(uName);
            var file = _service.Find(fName);
            if (file.ProcessorId == user.Id)
            {
                TempData["id"] = fName;
                return View("Upload");
            }
            else
            {
                TempData["MessageResult"] = "Cannot upload. You are not processor for this file.";
                return View("MessageResult");
            }
        }

        [EsAuthorize(Roles = "FileUpload")]
        [Audit(AuditingLevel = 0)]
        [HttpPost]
        public async Task<ActionResult> UploadFile(string id)
        {
            //Get application user for verification
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var file = _service.Find(id);

            //Only user who has the file assigned, can upload the file against it 
            if (file.ProcessorId == applicationUser.Id)
            {
                foreach (string upload in Request.Files)
                {
                    if (Request.Files[upload].FileName != "")
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/uploads/";
                        string filename = Path.GetFileName(Request.Files[upload].FileName);
                        string fileExtension = filename.Substring(filename.IndexOf("."));
                        filename = filename.Insert(filename.IndexOf("."), Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, User.TimeZone()));
                        filename = filename.Replace(":", "-");

                        Request.Files[upload].SaveAs(Path.Combine(path, filename));

                        //Convert Request.Files to byte array
                        byte[] fileData = null;
                        fileData = Data.Helper.StreamToByte(Request.Files[upload].InputStream);

                        //Calculate page number
                        int pageCount = CalculatePageCount(fileData, fileExtension);
                        file.NumberOfPagesSubmitted = pageCount;
                        file.Status = Data.Helper.FileStatus.Submitted.ToString();
                        
                        file.SubmittedFileName = filename;
                        file.SubmittedFilePath = "/App_Data/uploads/";
                        file.SubmittedDate = DateTime.UtcNow;
                        
                        _service.Update(file, applicationUser.Id, applicationUser.OrgId.ToString());
                        _unitOfWork.SaveChanges();
                    }
                }
            }
            else
            {
                TempData["MessageResult"] = "Cannot upload. You are not processor for this file.";
                return View("MessageResult");
            }

            return RedirectToAction("Index");
        }

        //public async Task<ActionResult> Downloads()
        //{
        //    var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/uploads/"));
        //    System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
        //    List<string> items = new List<string>();
        //    foreach (var file in fileNames)
        //    {
        //        items.Add(file.Name);
        //    }
        //    return View(items);
        //}

        [EsAuthorize(Roles = "FileDownload")]
        public FileResult DownloadFile(string id)
        {
            var file = _service.Find(id);
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            if (file.ProcessorId == applicationUser.Id)
            {
                //var FileVirtualPath = "~/App_Data/uploads/" + file.FileName;
                var FileVirtualPath = "~" + file.FilePath + file.FileName;
                return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
            }
            else
            {
                return File(new byte[0], "txt");
            }
        }

        [EsAuthorize(Roles = "FileDownload")]
        public FileResult DownloadTemplate(string id)
        {
            var file = _service.Find(id);
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            if (file.ProcessorId == applicationUser.Id)
            {
                //var FileVirtualPath = "~/App_Data/uploads/" + file.FileName;
                var FileVirtualPath = "~" + file.TemplateUrl;
                return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
            }
            else
            {
                return File(new byte[0], "txt");
            }
        }

        [EsAuthorize(Roles = "FileDownload")]
        public FileResult DownloadSubmittedFile(string id)
        {
            var file = _service.Find(id);
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

            if (file.ProcessorId == applicationUser.Id)
            {
                //var FileVirtualPath = "~/App_Data/uploads/" + file.FileName;
                var FileVirtualPath = "~" + file.FilePath + file.SubmittedFileName;
                return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
            }
            else
            {
                return File(new byte[0], "pdf");
            }
        }

        public async Task<ActionResult> Compose(string toEmailAddress = "")
        {
            TempData["toEmailAddress"] = toEmailAddress;
            return View();
        }

        public async Task<ActionResult> CVInbox()
        {
            List<EMailInbox> emails = null; // await GetEMailInbox();
            return PartialView(emails);
        }

        [EsAuthorize(Roles = "FileUpload, FileDownload, FileAuthorize, FileAdmin")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> EditPageCount(string id)
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

            return View(instance);
        }

        // POST: 
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [EsAuthorize(Roles = "FileUpload, FileDownload, FileAuthorize, FileAdmin")]
        [ValidateAntiForgeryToken]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> EditPageCount([Bind(Include = "Id,NumberOfPagesRecieved,NumberOfPagesSubmitted,ShrtDesc,Desc")] Lifetrons.Erp.Data.File instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                var model = await _service.FindAsync(instance.Id.ToString());
                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;
                
                model.NumberOfPagesRecieved = instance.NumberOfPagesRecieved;
                model.NumberOfPagesSubmitted = instance.NumberOfPagesSubmitted;
                model.ShrtDesc = instance.ShrtDesc;
                model.Desc = instance.Desc;

                model.Authorized = instance.Authorized;
                model.Active = true;
                ModelState.Clear();

                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        _unitOfWork.SaveChanges();

                        return RedirectToAction("Index");
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

                return View(instance);
            }

            return HttpNotFound();
        }


        [EsAuthorize(Roles = "FileAuthorize, FileAdmin")]
        [Audit(AuditingLevel = 0)]
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

            ViewBag.ContactId = new SelectList(await _contactService.SelectAsyncAccountNotNull(applicationUser.OrgId.ToString()), "Id", "Name", instance.ContactId);

            return View(instance);
        }

        // POST: 
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [EsAuthorize(Roles = "FileAuthorize, FileAdmin")]
        [ValidateAntiForgeryToken]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FileType,Status,NumberOfPagesRecieved,NumberOfPagesSubmitted,ContactId,Active,Authorized,Desc")] Lifetrons.Erp.Data.File instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                var model = await _service.FindAsync(instance.Id.ToString());
                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;
                model.FileType = instance.FileType;
                model.Status = instance.Status;
                model.NumberOfPagesRecieved = instance.NumberOfPagesRecieved;
                model.NumberOfPagesSubmitted = instance.NumberOfPagesSubmitted;
                model.Desc = instance.Desc;

                if (model.Status != Data.Helper.FileStatus.Rejected.ToString())
                {
                    model.ContactId = instance.ContactId;
                    //Fill up account from contact only
                    var contact = _contactService.Find(instance.ContactId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                    //File Rate Table
                    //Get the rates and templates
                    var fileRateTable = _fileRateTableService.FindByContact(model.FileType, model.ContactId.ToString(), DateTime.UtcNow, DateTime.UtcNow);
                    if (fileRateTable.Count() > 0)
                    {
                        var fileRate = fileRateTable.FirstOrDefault();
                        if (fileRate != null)
                        {
                            model.AccountId = contact.AccountId.ToSysGuid();
                            model.TemplateName = fileRate.TemplateName + Data.Helper.SysSeparator + fileRate.TemplateUpdateDate.ToString();//Template Name is just for display purpose. Example "TemplateV1"
                            model.TemplateUrl = fileRate.TemplateUrl; //Template url contains thename of file as well
                            model.RateType = fileRate.RateType;
                            model.Rate = fileRate.Rate;
                        }
                    }
                }

                model.Authorized = instance.Authorized;
                model.Active = true;
                ModelState.Clear();

                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        _unitOfWork.SaveChanges();

                        return RedirectToAction("JobsInQueue");
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

                return View(instance);
            }

            return HttpNotFound();
        }

        [EsAuthorize(Roles = "FileAuthorize, FileAdmin")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> EditRate(string id)
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
            instance.Contact = await _contactService.FindAsync(instance.ContactId.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

            ViewBag.FileRateTableId = new SelectList(_fileRateTableService.FindByContact(Data.Helper.FileType.CV.ToString(), instance.ContactId.ToString(), DateTime.UtcNow, DateTime.UtcNow), "Id", "RateType", instance.RateType);

            return View(instance);
        }

        // POST: 
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [EsAuthorize(Roles = "FileAuthorize, FileAdmin")]
        [ValidateAntiForgeryToken]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> EditRate([Bind(Include = "Id,RateType,Rate")] Lifetrons.Erp.Data.File instance, string FileRateTableId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                var model = await _service.FindAsync(instance.Id.ToString());
                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;
                model.Desc = model.Desc + " " + Data.Helper.SysSeparator + "Rate Modified";

                //File Rate Table
                //Get the rates and templates
                var fileRateTable = await _fileRateTableService.FindAsync(FileRateTableId);
                model.AccountId = fileRateTable.AccountId.ToSysGuid();
                model.TemplateName = fileRateTable.TemplateName + Data.Helper.SysSeparator + fileRateTable.TemplateUpdateDate.ToString();//Template Name is just for display purpose. Example "TemplateV1"
                model.TemplateUrl = fileRateTable.TemplateUrl; //Template url contains thename of file as well
                model.RateType = fileRateTable.RateType;
                model.Rate = fileRateTable.Rate;

                ModelState.Clear();

                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        _unitOfWork.SaveChanges();

                        return RedirectToAction("JobsInQueue");
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

                return View(instance);
            }

            return HttpNotFound();
        }

        [EsAuthorize(Roles = "FileAdmin")]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> EditProcessor(string id)
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

            ViewBag.ProcessorId = new SelectList(await _aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", instance.ProcessorId);

            return View(instance);
        }

        [HttpPost]
        [EsAuthorize(Roles = "FileAdmin")]
        [ValidateAntiForgeryToken]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> EditProcessor([Bind(Include = "Id,Status,ProcessorId,Desc")] Lifetrons.Erp.Data.File instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                var model = await _service.FindAsync(instance.Id.ToString());
                model.ObjectState = ObjectState.Modified;
                model.ModifiedBy = applicationUser.Id;
                model.ModifiedDate = DateTime.UtcNow;

                model.Status = instance.Status;
                model.ProcessorId = instance.ProcessorId;

                var processor = _aspNetUserService.Find(model.ProcessorId);
                model.ProcessorEmail = processor.AuthenticatedEmail;
                model.Desc = instance.Desc + " " + Data.Helper.SysSeparator + "Manually assigned to " + processor.AuthenticatedEmail + " on " + DateTime.UtcNow + "! ";

                ModelState.Clear();

                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        _unitOfWork.SaveChanges();

                        return RedirectToAction("JobsInQueue");
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

                return View(instance);
            }

            return HttpNotFound();
        }

        [Audit(AuditingLevel = 0)]
        private async Task<int> DownloadEmailsAndCreateFilesQueue(AspNetUser user = null)
        {
            if (user == null)
            {
                user = _aspNetUserService.Find(Data.Helper.SupportUserId);
            }

            var recentFile = _service.FindRecentFile(Helper.FileType.CV.ToString(), user.OrgId.ToString());
            EmailConfig instance = _emailConfigService.Find(Data.Helper.EmailConfigIdForFilesOps);
            var pop3Client = new Pop3Client();
            try
            {
                //pop3Client.Connect("pop.gmail.com", 995, true);
                //pop3Client.Authenticate("msb.net.in@gmail.com", "pass", AuthenticationMethod.Auto);
                pop3Client.Connect(instance.Pop3Host, Convert.ToInt32(instance.Pop3Port), instance.Ssl);
                pop3Client.Authenticate(instance.Pop3Username, instance.Pop3Password, AuthenticationMethod.Auto);
            }
            catch (Exception)
            {
                TempData["MessageResult"] = "Email service busy! Please try after some time!!";
                //return View("MessageResult");
                return 0;
            }

            int count = pop3Client.GetMessageCount();
            var messages = new List<Message>();
            int counter = 0;
            int filesCounter = 0;
            for (int i = count; i >= 1; i--) //For EasySales.in
                                             //for (int i = 0; i >= 3; i++) // For Gmail
            {
                Message message = pop3Client.GetMessage(i);
                if (recentFile != null)//For empty table
                {
                    if (message.Headers.DateSent == recentFile.FileSentDate && message.Headers.Subject == recentFile.RcvdFileEmailSubject)
                    {
                        break;
                    }
                }

                messages.Add(message);
                counter++;
                //if (counter > 10) { break; }
            }
            pop3Client.Disconnect();
            pop3Client.Dispose();

            //Create File Objects from this message
            var files = await CreateFileObjectFromEmail(messages, user);
            //var files = CreateFileObjectFromEmail(messages, user);
            //Count of file jobs created from this message
            filesCounter += files.Count();

            return filesCounter;
        }

        [Audit(AuditingLevel = 0)]
        private async Task<List<Lifetrons.Erp.Data.File>> CreateFileObjectFromEmail(List<Message> messages, AspNetUser user)
        {
            List<Lifetrons.Erp.Data.File> files = new List<Data.File>();

            foreach (var message in messages)
            {
                List<MessagePart> attachments = message.FindAllAttachments();
                //Create a job for each valid attachment
                foreach (MessagePart attachment in attachments)
                {
                    //For document files
                    if (Helper.ValidExtensionForFileJob(Helper.FileType.CV.ToString(), attachment.FileName))
                    {
                        Lifetrons.Erp.Data.File objFile = new Lifetrons.Erp.Data.File();
                        objFile.OriginSenderEmail = message.Headers.From.Address;

                        foreach (var item in message.Headers.Cc)
                        {
                            objFile.OriginCCEmails = item.Address + ",";
                        }

                        objFile.Origin = message.Headers.From.DisplayName;
                        objFile.RcvdFileEmailSubject = message.Headers.Subject;

                        MessagePart body = message.FindFirstHtmlVersion();
                        if (body != null)
                        {
                            objFile.RcvdFileEmailBody = body.GetBodyAsText();
                        }
                        else
                        {
                            body = message.FindFirstPlainTextVersion();
                            if (body != null)
                            {
                                objFile.RcvdFileEmailBody = body.GetBodyAsText();
                            }
                        }

                        HtmlDocument htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(objFile.RcvdFileEmailBody);
                        //HTML removed from body
                        objFile.RcvdFileEmailBody = htmlDoc.DocumentNode.InnerText;
                        //Reduce the length of body
                        if (objFile.RcvdFileEmailBody.Length > 301)
                        {
                            objFile.RcvdFileEmailBody = objFile.RcvdFileEmailBody.Substring(0, 300);
                        }

                        objFile.FileSentDate = message.Headers.DateSent;
                        objFile.FileExtension = attachment.FileName.Substring(attachment.FileName.IndexOf("."));
                        objFile.FileType = Helper.FileType.CV.ToString(); //attachment.ContentType.MediaType
                        objFile.FileName = attachment.FileName.Insert(attachment.FileName.IndexOf("."), Helper.SysSeparator + ControllerHelper.ConvertDateTimeFromUtc(DateTime.UtcNow, User.TimeZone()));
                        objFile.FileName = objFile.FileName.Replace(":", "-");

                        //Write stream in server
                        objFile.FilePath = "/App_Data/uploads/";
                        string path = AppDomain.CurrentDomain.BaseDirectory + objFile.FilePath;
                        System.IO.File.WriteAllBytes(path + objFile.FileName, attachment.Body);
                        //Calculate page number
                        int pageCount = CalculatePageCount(attachment.Body, objFile.FileExtension);
                        objFile.NumberOfPagesRecieved = pageCount;

                        objFile.BackupEmail = Data.Helper.BackupEmailForFilesOps;
                        //Find contact to extract account info for rates
                        var contacts = _contactService.FindByEmail(message.Headers.From.Address);
                        if (contacts != null)
                        {
                            if (contacts.Count() > 0)
                            {
                                var contact = contacts.FirstOrDefault();
                                if (contact != null)
                                {
                                    objFile.ContactId = contact.Id;
                                    objFile.AccountId = contact.AccountId;
                                    objFile.Status = Helper.FileStatus.Queued.ToString();
                                    //Get the rates and templates
                                    var fileRateTable = _fileRateTableService.FindByContact(objFile.FileType, objFile.ContactId.ToString(), DateTime.UtcNow, DateTime.UtcNow);
                                    if (fileRateTable.Count() > 0)
                                    {
                                        foreach (var fileRate in fileRateTable)
                                        {
                                            // If Non urgent detected then assign and break loop
                                            if (fileRate.RateType == Data.Helper.FileRateType.PerPageNonUrgent.ToString() ||
                                            fileRate.RateType == Data.Helper.FileRateType.PerFileNonUrgent.ToString())
                                            {
                                                // If Non urgent detected then assign and break loop
                                                if (objFile.RcvdFileEmailSubject.Contains("Non Urgent") ||
                                                    objFile.RcvdFileEmailSubject.Contains("Non-Urgent") ||
                                                    objFile.RcvdFileEmailSubject.Contains("NonUrgent"))
                                                {
                                                    objFile.RateType = fileRate.RateType;
                                                    objFile.Rate = fileRate.Rate;
                                                    //Template url contains the name of file as well. Template Name is just for display purpose. Example "TemplateV1"
                                                    objFile.TemplateUrl = fileRate.TemplateUrl;
                                                    objFile.TemplateName = fileRate.TemplateName + Data.Helper.SysSeparator + fileRate.TemplateUpdateDate.ToString();
                                                    break;
                                                }
                                            }
                                            else // Assign Urgent Rate Type by default and continue with loop
                                            {
                                                objFile.RateType = fileRate.RateType;
                                                objFile.Rate = fileRate.Rate;
                                                //Template url contains the name of file as well. Template Name is just for display purpose. Example "TemplateV1"
                                                objFile.TemplateUrl = fileRate.TemplateUrl;
                                                objFile.TemplateName = fileRate.TemplateName + Data.Helper.SysSeparator + fileRate.TemplateUpdateDate.ToString();
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //Change the status to "Review" if no Status assigned until now
                        if (objFile.Status == null)
                        {
                            objFile.Status = Data.Helper.FileStatus.Review.ToString();
                        }


                        //Add the Guid
                        objFile.Id = Guid.NewGuid();
                        objFile.OrgId = user.OrgId.ToSysGuid();
                        objFile.CreatedBy = user.Id;
                        objFile.CreatedDate = DateTime.UtcNow;
                        objFile.ModifiedBy = user.Id;
                        objFile.ModifiedDate = DateTime.UtcNow;
                        objFile.Authorized = true;
                        objFile.Active = true;
                        //The the file object to collection
                        files.Add(objFile);
                    }

                } // attachment loop ends
            } // Main Message loop ends
            // Update database
            foreach (var file in files)
            {
                _service.Create(file, Data.Helper.SupportUserId, Data.Helper.SupportUserOrgId);
                _unitOfWork.SaveChanges();
            }

            return files;
        }

        private async Task<List<EMailInbox>> GetEMailInbox()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            EmailConfig instance = _emailConfigService.Find(Data.Helper.EmailConfigIdForFilesOps);

            var pop3Client = new Pop3Client();
            //pop3Client.Connect("pop.gmail.com", 995, true);
            //pop3Client.Authenticate("msb.net.in@gmail.com", "pass", AuthenticationMethod.Auto);
            pop3Client.Connect(instance.Pop3Host, Convert.ToInt32(instance.Pop3Port), instance.Ssl);
            pop3Client.Authenticate(instance.Pop3Username, instance.Pop3Password, AuthenticationMethod.Auto);

            int count = pop3Client.GetMessageCount();
            var inbox = new List<EMailInbox>();
            int counter = 0;
            for (int i = count; i >= 1; i--)
            {
                Message message = pop3Client.GetMessage(i);
                var email = new EMailInbox
                {
                    MessageNumber = i,
                    Subject = message.Headers.Subject,
                    DateSent = message.Headers.DateSent,
                    //From = string.Format("<a href = 'mailto:{1}'>{0}</a>", message.Headers.From.DisplayName, message.Headers.From.Address),
                    From = string.Format("{0} <{1}>", message.Headers.From.DisplayName, message.Headers.From.Address),
                };

                MessagePart body = message.FindFirstHtmlVersion();
                if (body != null)
                {
                    email.Body = body.GetBodyAsText();
                }
                else
                {
                    body = message.FindFirstPlainTextVersion();
                    if (body != null)
                    {
                        email.Body = body.GetBodyAsText();
                    }
                }

                List<MessagePart> attachments = message.FindAllAttachments();
                foreach (MessagePart attachment in attachments)
                {
                    email.Attachments.Add(new Lifetrons.Erp.Data.Attachment
                    {
                        FileName = attachment.FileName,
                        ContentType = attachment.ContentType.MediaType,
                        Content = attachment.Body
                    });
                }

                inbox.Add(email);
                counter++;
                if (counter > 10)
                {
                    break;
                }

            }

            return inbox;
        }

        private int CalculatePageCount(byte[] fileData, string extension)
        {
            int pageCount = 0;
            try
            {
                if (extension.Contains("pdf"))
                {
                    //string ppath = "C:\\aworking\\Hawkins.pdf";
                    //PdfReader pdfReader = new PdfReader(ppath);

                    PdfReader pdfReader = new PdfReader(fileData);
                    pageCount = pdfReader.NumberOfPages;

                    return pageCount;
                }
                //else if(extension.Contains("doc"))
                //{
                //    var application = new Application();
                //    //var document = application.Documents.Open(@"C:\Users\MyName\Documents\word.docx");
                //    var document = application.Documents.Open(fileData);

                //    // Get the page count.
                //    var numberOfPages = document.ComputeStatistics(WdStatistic.wdStatisticPages, false);
                //    pageCount = numberOfPages;

                //    // Close word.
                //    application.Quit();

                //    return pageCount;
                //}
                else if (extension.Contains("docx"))
                {
                    Stream stream = new MemoryStream(fileData);
                    DocumentFormat.OpenXml.Packaging.WordprocessingDocument doc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(stream, false);
                    string numberofPages = doc.ExtendedFilePropertiesPart.Properties.Pages.InnerText.ToString();
                    pageCount = int.Parse(numberofPages);
                }
                else if (extension.Contains("doc") || extension.Contains("rtf"))
                {
                    try
                    {
                        //using (StreamReader sr = new StreamReader(System.IO.File.OpenRead(file)))
                        using (StreamReader sr = new StreamReader(new MemoryStream(fileData)))
                        {
                            Regex regex = new Regex(@"/Type\s*/Page[^s]");
                            MatchCollection matches = regex.Matches(sr.ReadToEnd());
                            pageCount = matches.Count;

                            return pageCount;
                        }
                    }
                    catch (Exception ex)
                    {
                        return pageCount;
                    }
                }
            }
            catch (Exception ex)
            {

                return pageCount;
            }


            return pageCount;
        }

        [HttpGet]
        [EsAuthorize(Roles = "FileAuthorize, FileUpload, FileDownload")]
        public async Task<ActionResult> DispatchFile(string fileId, string subject = "", string message = "")
        {
            var file = _service.Find(fileId);
            if (file == null)
            {
                TempData["MessageResult"] = "Not Valid!!";
                return View("MessageResult");
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            EmailConfig mailConfig = _emailConfigService.Find(Data.Helper.EmailConfigIdForFilesOps);

            file.DeliveredFileEmailSubject = string.IsNullOrEmpty(subject) ? file.SubmittedFileName : subject;
            file.SubmittedFileEmailBody = string.IsNullOrEmpty(message) ? "Hi, <br /><br />Pls find the processed subject CV attached herewith. <br /> <br /> Kind regards <br /> <br /> " + Helper.SysSeparator + DateTime.UtcNow : message;
            var submittedFile = DownloadSubmittedFile(fileId);
            var FileVirtualPath = "~" + file.FilePath + file.SubmittedFileName;

            //string path = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/uploads/";
            Stream attachment = System.IO.File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + file.FilePath + file.SubmittedFileName);

            //Prepare attachments
            var attachmentsDictionary = new Dictionary<string, Stream>();
            attachmentsDictionary.Add(file.SubmittedFileName, attachment);

            try
            {
                var ccList = "";
                if (string.IsNullOrEmpty(file.OriginCCEmails))
                {
                    ccList = file.BackupEmail;
                }
                else
                {
                    ccList = file.OriginCCEmails.EndsWith(",") ? file.BackupEmail : "," + file.BackupEmail;
                }

                bool result = await _emailConfigService.SendMail(mailConfig.UserId, applicationUser.OrgId.ToString(), file.OriginSenderEmail, ccList, file.DeliveredFileEmailSubject, file.SubmittedFileEmailBody, false, attachmentsDictionary);
                if (result)
                {
                    try
                    {
                        file.DeliveredDate = DateTime.UtcNow;
                        file.Status = Data.Helper.FileStatus.Delivered.ToString();
                        _service.Update(file, applicationUser.Id, applicationUser.OrgId.ToString());
                        _unitOfWork.SaveChanges();

                        TempData["MessageResult"] = "Your message was sent successfully!";
                        return View("MessageResult");
                    }
                    catch (Exception ex)
                    {

                        throw new Exception("Dispatch File returned false without exception." + ex.Message);
                    }
                }
                else
                {
                    TempData["MessageResult"] = "Email service did not respond. Please try after some time!";
                    return View("MessageResult");
                }
            }
            catch (Exception exception)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
                TempData["MessageResult"] = "Email service did not respond. Please try after some time!There was an error... please try again. \n" + exception.Message;
                return View("MessageResult");
            }
        }

        private async void RunJob()
        {
            if (!IsDownloadEmailsJobRunning)
            {
                if (DateTime.UtcNow > DtDownloadEmailsJob.AddMinutes(10))
                {
                    IsDownloadEmailsJobRunning = true;
                    DtDownloadEmailsJob = DateTime.UtcNow;
                    this.JobsCreatedByDownloadEmailsRunJob = await DownloadEmailsAndCreateFilesQueue();
                    IsDownloadEmailsJobRunning = false;
                }
            }
        }
    }
}