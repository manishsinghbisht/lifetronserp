using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using OpenPop.Mime;
using OpenPop.Pop3;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using WebGrease.Css.Extensions;

namespace Lifetrons.Erp.Controllers
{
    public class EmailController : BaseController
    {
        private readonly IEmailConfigService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAspNetUserService _aspNetUserService;

        public EmailController(IEmailConfigService service, IUnitOfWorkAsync unitOfWork, IAspNetUserService aspNetUserService)
        {
            _service = service;
            _unitOfWork = unitOfWork;
            _aspNetUserService = aspNetUserService;
        }

        // GET: /Email/Create
        [EsAuthorize]
        public async Task<ActionResult> OpenEmailConfiguration()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            EmailConfig instance = await _service.FindAsyncFirstOrDefault(applicationUser.Id, applicationUser.OrgId.ToString());

            if (instance == null)
            {
                return RedirectToAction("Create");
            }

            return RedirectToAction("Edit", new { id = instance.Id });
        }

        // GET: /Email/Create
        [EsAuthorize]
        public async Task<ActionResult> Create()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            return View();
        }

        // POST: /Email/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize]
        public async Task<ActionResult> Create([Bind(Include = "Name,SmtpUsername,SmtpPassword,SmtpHost,SmtpPort,Pop3Username,Pop3Password,Pop3Host,Pop3Port,Ssl,Tls,HeskSettings,OtherSettings,IsPrimary,Authorized")] EmailConfig instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());

                instance.Id = Guid.NewGuid();
                instance.OrgId = applicationUser.OrgId.ToSysGuid();
                instance.UserId = applicationUser.Id;
                instance.IsPrimary = true;
                instance.Authorized = true;
                instance.Active = true;

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

            return RedirectToAction("Compose");
        }

        // GET: /Email/Edit/5
        [EsAuthorize]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            EmailConfig instance = await _service.FindAsync(id, applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

            return View(instance);
        }

        // POST: /Case/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EsAuthorize]
        [Audit(AuditingLevel = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,SmtpUsername,SmtpPassword,SmtpHost,SmtpPort,Pop3Username,Pop3Password,Pop3Host,Pop3Port,Ssl,Tls,HeskSettings,OtherSettings,IsPrimary,Authorized")] EmailConfig instance)
        {
            if (User.Identity.IsAuthenticated)
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                EmailConfig model = await _service.FindAsync(instance.Id.ToString(), applicationUser.Id, applicationUser.OrgId.ToString());

                model.ObjectState = ObjectState.Modified;

                instance.IsPrimary = instance.IsPrimary;
                instance.Authorized = true;
                instance.Active = true;

                model.Name = instance.Name;
                model.SmtpUsername = instance.SmtpUsername;
                model.SmtpPassword = instance.SmtpPassword;
                model.SmtpHost = instance.SmtpHost;
                model.SmtpPort = instance.SmtpPort;
                model.Pop3Username = instance.Pop3Username;
                model.Pop3Password = instance.Pop3Password;
                model.Pop3Host = instance.Pop3Host;
                model.Pop3Port = instance.Pop3Port;
                model.Ssl = instance.Ssl;
                model.Tls = instance.Tls;
                model.HeskSettings = instance.HeskSettings;
                model.OtherSettings = instance.OtherSettings;

                ModelState.Clear();
                try
                {
                    if (TryValidateModel(model))
                    {
                        _service.Update(model, applicationUser.Id, applicationUser.OrgId.ToString());
                        await _unitOfWork.SaveChangesAsync();

                        return RedirectToAction("Compose");
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


        [Authorize]
        public async Task<ActionResult> Compose(string toEmailAddress = "")
        {
            TempData["toEmailAddress"] = toEmailAddress;
            return View();
        }

        [Authorize]
        public async Task<ActionResult> Sent()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SendMail(string toEmail, string ccEmail, string subject, string message)
        {
            if (toEmail == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
                bool result = await _service.SendMail(applicationUser.Id, applicationUser.OrgId.ToString(), toEmail, ccEmail, subject, message, false, null);
                if (result)
                {
                    return Content("Your message was sent successfully!");
                }
                throw new Exception("SendEmail returned false without exception.");
            }
            catch (Exception exception)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
                return Content("There was an error... please try again. \n" + exception.InnerException.Message);
            }
        }

       [HttpGet]
        public async Task<ActionResult> EmailInbox()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            EmailConfig instance = await _service.FindAsyncFirstOrDefault(applicationUser.Id, applicationUser.OrgId.ToString());
            if (instance == null)
            {
                return HttpNotFound();
            }

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

                //MessagePart body = message.FindFirstHtmlVersion();
                //if (body != null)
                //{
                //    email.Body = body.GetBodyAsText();
                //}
                //else
                //{
                //    body = message.FindFirstPlainTextVersion();
                //    if (body != null)
                //    {
                //        email.Body = body.GetBodyAsText();
                //    }
                //}
                //List<MessagePart> attachments = message.FindAllAttachments();

                //foreach (MessagePart attachment in attachments)
                //{
                //    email.Attachments.Add(new Lifetrons.Erp.Data.Attachment
                //    {
                //        FileName = attachment.FileName,
                //        ContentType = attachment.ContentType.MediaType,
                //        Content = attachment.Body
                //    });
                //}

                inbox.Add(email);
                counter++;
                if (counter > 10)
                {
                    break;
                }
            }
            return PartialView(inbox);
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