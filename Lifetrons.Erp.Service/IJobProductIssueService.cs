using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Repository.Models;

namespace Lifetrons.Erp.Service
{
    public interface IJobProductIssueService
    {
        IEnumerable<JobProductIssue> SelectLineItems(string jobIssueId, string userId, string orgId);
        Task<IEnumerable<JobProductIssue>> SelectAsyncLineItems(string jobIssueId, string userId, string orgId);
        Task<JobProductIssue> FindAsync(string id, string userId, string orgId);
        JobProductIssue Find(string id, string userId, string orgId);
        JobProductIssue Create(JobProductIssue param, string userId, string orgId);
        void Update(JobProductIssue param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<JobProductIssue> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        Task<IEnumerable<JobProductIssue>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId);
        decimal TotalQuantityIssued(string jobNo, string issuedToProcessId, string userId, string orgId);

        decimal TotalQuantityIssued(string jobNo, string issuedByProcessId, string issuedToProcessId, string userId,
            string orgId);
        
        IEnumerable<JobQuantityTotals> ProcesswiseQuantitiesGroupByJobs(DateTime startDateTime, DateTime endDateTime,
            string issuedByProcessId, string issuedToProcessId, string userId, string orgId);

        decimal ProcessLoadOut(DateTime startDateTime, string issuedByProcessId, string orgId);
        decimal ProcessLoadIn(DateTime startDateTime, string issuedToProcessId, string orgId);

        IEnumerable<JobQuantityTotals> ProcessLoadInJobwise(DateTime startDateTime, string issuedToProcessId, string orgId);
        IEnumerable<JobQuantityTotals> ProcessLoadOutJobwise(DateTime startDateTime, string issuedByProcessId, string orgId);
        decimal AssemblyLoadOut(DateTime startDateTime, string orgId);
        IEnumerable<JobQuantityTotals> AssemblyLoadOutJobwise(DateTime startDateTime, string orgId);
        void Dispose();
    }
}