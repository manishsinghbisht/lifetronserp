using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IJobIssueHeadService
    {
        IEnumerable<JobIssueHead> Select(string id, string userId, string orgId);
        Task<IEnumerable<JobIssueHead>> SelectAsync(string userId, string orgId);
        JobIssueHead Find(string id, string userId, string orgId);
        Task<JobIssueHead> FindAsync(string id, string userId, string orgId);
        JobIssueHead Create(JobIssueHead param, string userId, string orgId);
        void Update(JobIssueHead param, string userId, string orgId);
        void Delete(JobIssueHead model);
        IEnumerable<JobIssueHead> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);

        Task<IEnumerable<JobIssueHead>> SelectAsyncByDate(DateTime startDate, DateTime endDate, string userId, string orgId);
        Task<IEnumerable<JobIssueHead>> SelectAsyncIssuedToProcessIdByDate(DateTime startDate, DateTime endDate, string issuedToProcessId, string userId, string orgId);
        Task<IEnumerable<JobIssueHead>> SelectAsyncIssuedByProcessByDate(DateTime startDate, DateTime endDate, string issuedByProcessId, string userId, string orgId);
        void Dispose();
    }
}