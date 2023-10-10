using System.Net;
using System.Net.Mail;
using Lifetrons.Erp.Data;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenPop.Mime;
using OpenPop.Pop3;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System.Configuration;
using Microsoft.Ajax.Utilities;
using System.IO;

namespace Lifetrons.Erp.Service
{
    public class EmailConfigService : Service<EmailConfig>, IEmailConfigService
    {
        private readonly IRepositoryAsync<EmailConfig> _repository;
        private readonly IAspNetUserService _aspNetUserService;
        private string SupportUserId = Data.Helper.SupportUserId;

        public EmailConfigService(IRepositoryAsync<EmailConfig> repository, IAspNetUserService aspNetUserService)
            : base(repository)
        {
            _repository = repository;
            _aspNetUserService = aspNetUserService;
        }

        public async Task<IEnumerable<EmailConfig>> GetAsync(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var task = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.UserId == userId)
              .Include(p => p.AspNetUser)
              .SelectAsync();

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<EmailConfig> FindAsyncFirstOrDefault(string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var task = await _repository.Query(p => p.Active & p.OrgId == orgIdGuid & p.UserId == userId)
              .Include(p => p.AspNetUser)
              .SelectAsync();

            if (task == null) return null;

            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (orgIdGuid == applicationUser.OrgId)
            {
                return task.FirstOrDefault();
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public async Task<EmailConfig> FindAsyncFirstOrDefaultByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var task = await _repository.Query(p => p.Active & p.Name == name)
              .Include(p => p.AspNetUser)
              .SelectAsync();

            if (task == null) return null;
            return task.FirstOrDefault();
        }

        public EmailConfig FindFirstOrDefaultByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var task = _repository.Query(p => p.Active & p.Name == name)
              .Include(p => p.AspNetUser)
              .Select();

            if (task == null) return null;
            return task.FirstOrDefault();
        }

        public EmailConfig FindFirstOrDefaultByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            var task = _repository.Query(p => p.Active & p.UserId == userId)
              .Include(p => p.AspNetUser)
              .Select();

            if (task == null) return null;
            return task.FirstOrDefault();
        }

        private async Task<EmailConfig> GetSupportEmailAsync()
        {
            var task = await _repository.Query(p => p.Active & p.UserId == SupportUserId)
               .Include(p => p.AspNetUser)
               .SelectAsync();

            return task.First();
        }

        public async Task<EmailConfig> FindAsync(string id, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            var task = await _repository.FindAsync(id.ToSysGuid());
            //Check user & org
            var applicationUser = await _aspNetUserService.FindAsync(userId);
            if (task == null) return task;
            if (task.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                return task;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public EmailConfig Create(EmailConfig param, string userId, string orgId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId)) return null;

            Guid orgIdGuid = orgId.ToSysGuid();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                _repository.Insert(param);
                return param;
            }

            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }

        public void Update(EmailConfig param, string userId, string orgId)
        {
            if (param == null || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(orgId))
                throw new ApplicationException("Parameters cannot be null or empty");

            Guid orgIdGuid = orgId.ToSysGuid();

            //Check user & org
            var applicationUser = _aspNetUserService.Find(userId);
            if (param.OrgId == orgIdGuid && orgIdGuid == applicationUser.OrgId)
            {
                //Update
                _repository.Update(param);
            }
            else
            {
                throw new ApplicationException("Data not found", new Exception("Organization did not match."));
            }
        }

        public EmailConfig Find(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var record = _repository.Find(id.ToSysGuid());

            if (record == null) return null;

            return record;
        }

        //public void Delete(string id)
        //{
        //    //Do the validation here

        //    //Do business logic here

        //    _repository.Delete(id.ToSysGuid());
        //}

        public IEnumerable<EmailConfig> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId)
        {
            Guid orgIdGuid = orgId.ToSysGuid();

            var applicationUser = _aspNetUserService.Find(userId);
            if (applicationUser.OrgId == orgIdGuid)
            {
                var records = _repository
                    .Query(q => !string.IsNullOrEmpty(q.UserId) & q.OrgId.Equals(orgIdGuid))
                    .OrderBy(q => q
                        .OrderBy(c => c.UserId)
                        .ThenBy(c => c.SmtpUsername))
                    .SelectPage(pageNumber, pageSize, out totalRecords);

                return records;
            }
            throw new ApplicationException("Data not found", new Exception("Organization did not match."));
        }
        public void Dispose()
        {
        }

        public async Task<bool> SendMail(string emailSenderUserId, string orgId, string toEmail, string ccEmail, string subject, string message, bool canUseSystemEmailForSending = false, Dictionary<string, Stream> attachments = null)
        {
            Exception userEmailException;
            Exception smtpException;
            Exception supportEmailException;

            //Validate input parameters and get app user
            var emailSender = await GetEMailSender(emailSenderUserId, orgId, toEmail);

            //Get Email configuration of user or support user
            var instance = await EmailConfig(canUseSystemEmailForSending, emailSender);

            string fromEmail;
            string fromName;
            MailMessage msg;
            SmtpClient smtpClient;
            string compositeMessage;
            ConfigureEmail(message, instance, out fromEmail, out fromName, out msg, out smtpClient, out compositeMessage);

            try
            {
                bool mailSent = false;
                mailSent = CoreSendMail(toEmail, ccEmail, subject, msg, fromEmail, fromName, compositeMessage, smtpClient, out userEmailException, attachments);

                if (!mailSent)
                {
                    // When using support mail id in the first attempt itself.
                    if (instance.UserId == SupportUserId)
                    {
                        smtpException = new Exception("Sending failed from support email, email sending failed. " + "/n" + "Support email expcetion: " + userEmailException.Message);
                        throw smtpException;

                    }
                    if (canUseSystemEmailForSending)
                    {
                        //Mail sending from user mails failed. Try to send email via Support Email credentials. This will be the case when Use email configuration is present but password or setting are wrong.
                        instance = await GetSupportEmailAsync();
                        ConfigureEmail(message, instance, out fromEmail, out fromName, out msg, out smtpClient, out compositeMessage);
                        mailSent = CoreSendMail(toEmail, ccEmail, subject, msg, fromEmail, fromName, compositeMessage, smtpClient, out supportEmailException, attachments);
                        smtpException =
                            mailSent ? new Exception("Sending failed from user email, email sent through support mail. " + "/n" + "User email expcetion: " + userEmailException.Message)
                            : new Exception("Email sending failed in both user and support mail attempts, email sending failed. " + "/n" + "User email expcetion: " + userEmailException.Message + "/n" + "Support email expcetion: " + supportEmailException.Message);
                        throw smtpException;
                    }
                    else
                    {
                        smtpException = new Exception("Sending failed from user email, email sending failed. " + "/n" + "User email expcetion: " + userEmailException.Message);
                        throw smtpException;
                    }
                }
                //else if (instance.UserId != SupportUserId && fromEmail == "support@EasySales.in") // Case when mail sent in first attempt but by support mail id. This will be the case when no Email config record is present for user.
                //{
                //    smtpException = new Exception("User email exception: Sending failed from user email. Mail sent through support mail.");
                //    throw smtpException;
                //}

                //return true only when mail sent by user email
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in mail configuration. \n" + ex.Message);
            }
        }

        private static string ConfigureEmail(string message, EmailConfig instance, out string fromEmail, out string fromName, out MailMessage msg,
            out SmtpClient smtpClient, out string compositeMessage)
        {
            fromName = instance.Name;
            fromEmail = instance.SmtpUsername;
            string password = instance.SmtpPassword;
            var loginInfo = new NetworkCredential(fromEmail, password);
            msg = new MailMessage();

            //var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient = new SmtpClient(instance.SmtpHost, Convert.ToInt32(instance.SmtpPort))
            {
                EnableSsl = instance.Ssl,
                //UseDefaultCredentials = false, //default is false. So neet to set it. By setting it to false, we are sending the username / password with credential object.
                Credentials = loginInfo
            };

            if(string.IsNullOrEmpty(message))
            {
                compositeMessage = "Hello, <br /><br />" + fromName + " (" + fromEmail + ") has a message for you :<br /><br />" + message;
            }
            else
            {
                compositeMessage = message;
            }
            
            return fromName;
        }

        private async Task<AspNetUser> GetEMailSender(string emailSenderUserId, string orgId, string toEmail)
        {
            if (toEmail.IsNullOrWhiteSpace() || emailSenderUserId.IsNullOrWhiteSpace() || orgId.IsNullOrWhiteSpace())
            {
                throw new ApplicationException("Invalid input parameters.");
            }

            Guid orgIdGuid = orgId.ToSysGuid();

            //Check user & org
            var emailSender = await _aspNetUserService.FindAsync(emailSenderUserId);
            if (orgIdGuid != emailSender.OrgId)
                throw new ApplicationException("Data not found", new Exception("Organization did not match."));
            return emailSender;
        }

        private async Task<EmailConfig> EmailConfig(bool canUseSystemEmailForSending, AspNetUser emailSender)
        {
            EmailConfig instance = await FindAsyncFirstOrDefault(emailSender.Id, emailSender.OrgId.ToString());
            if (instance == null)
            {
                if (canUseSystemEmailForSending)
                {
                    //Try to find support email id and use it for sending mail
                    instance = await GetSupportEmailAsync();
                    if (instance == null) throw new ApplicationException("System support email configuration not found.");
                }
                else
                {
                    throw new ApplicationException("System support email configuration not found.");
                }
            }
            return instance;
        }

        private static bool CoreSendMail(string toEmail, string ccEmail, string subject, MailMessage msg, string fromEmail,
            string fromName, string compositeMessage, SmtpClient smtpClient, out Exception exception, Dictionary<string, Stream> attachments = null)
        {
            try
            {
                msg.From = new MailAddress(fromEmail, fromName);
                var toEmailList = toEmail.Split(new[] { ',' });
                toEmailList.ForEach(p => msg.To.Add(new MailAddress(p)));
                //msg.To.Add(new MailAddress(toEmail));
                //msg.CC.Add(new MailAddress(fromEmail));
                if (!ccEmail.IsNullOrWhiteSpace())
                {
                    var ccEmailList = ccEmail.Split(new[] { ',' });
                    ccEmailList.ForEach(cc => msg.To.Add(new MailAddress(cc)));
                }
                msg.Subject = subject;
                msg.Body = compositeMessage + " <br /><br />" + "(" + Data.Helper.AppName + ")";
                if(attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        msg.Attachments.Add(new System.Net.Mail.Attachment(attachment.Value, attachment.Key));
                    }
                }
                
                msg.IsBodyHtml = true;

                smtpClient.Send(msg);

                smtpClient.Dispose();
                exception = null;
                return true; // Your message was sent successfully!
            }
            catch (Exception ex)
            {
                smtpClient.Dispose();
                exception = ex;
                return false; // Your message was not sent successfully!
            }

        }
    }
}
