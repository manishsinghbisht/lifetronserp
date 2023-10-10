using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IJobItemIssueService
    {
        IEnumerable<JobItemIssue> SelectLineItems(string jobIssueId, string userId, string orgId);
        Task<IEnumerable<JobItemIssue>> SelectAsyncLineItems(string jobIssueId, string userId, string orgId);
        Task<JobItemIssue> FindAsync(string id, string userId, string orgId);
        JobItemIssue Find(string id, string userId, string orgId);
        JobItemIssue Create(JobItemIssue param, string userId, string orgId);
        void Update(JobItemIssue param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<JobItemIssue> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        Task<IEnumerable<JobItemIssue>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId);
        void Dispose();
    }
}