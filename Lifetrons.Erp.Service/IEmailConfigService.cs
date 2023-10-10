using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;
using System.IO;

namespace Lifetrons.Erp.Service
{
    public interface IEmailConfigService
    {
        Task<IEnumerable<EmailConfig>> GetAsync(string userId, string orgId);
        Task<bool> SendMail(string emailSenderUserId, string orgId, string toEmail, string ccEmail, string subject, string message, bool canUseSystemEmailForSending = false, Dictionary<string, Stream> attachments = null);
        Task<EmailConfig> FindAsync(string id, string userId, string orgId);
        Task<EmailConfig> FindAsyncFirstOrDefault(string userId, string orgId);
        Task<EmailConfig> FindAsyncFirstOrDefaultByName(string name);
        EmailConfig Create(EmailConfig param, string userId, string orgId);
        void Update(EmailConfig param, string userId, string orgId);
        IEnumerable<EmailConfig> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
        EmailConfig FindFirstOrDefaultByName(string name);
        EmailConfig FindFirstOrDefaultByUserId(string userId);
        EmailConfig Find(string id);
    }
}