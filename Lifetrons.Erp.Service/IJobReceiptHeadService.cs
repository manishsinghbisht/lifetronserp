using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IJobReceiptHeadService
    {
        IEnumerable<JobReceiptHead> Select(string id, string userId, string orgId);
        Task<IEnumerable<JobReceiptHead>> SelectAsync(string userId, string orgId);
        JobReceiptHead Find(string id, string userId, string orgId);
        Task<JobReceiptHead> FindAsync(string id, string userId, string orgId);
        JobReceiptHead Create(JobReceiptHead param, string userId, string orgId);
        void Update(JobReceiptHead param, string userId, string orgId);
        void Delete(JobReceiptHead model);
        IEnumerable<JobReceiptHead> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);

        Task<IEnumerable<JobReceiptHead>> SelectAsyncByDate(DateTime startDate, DateTime endDate, string userId, string orgId);
        Task<IEnumerable<JobReceiptHead>> SelectAsyncReceiptFromProcessIdByDate(DateTime startDate, DateTime endDate, string receiptFromProcessId, string userId, string orgId);
        Task<IEnumerable<JobReceiptHead>> SelectAsyncReceiptByProcessIdByDate(DateTime startDate, DateTime endDate, string receiptByProcessId, string userId, string orgId);
        void Dispose();
    }
}