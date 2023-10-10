using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Repository.Models;

namespace Lifetrons.Erp.Service
{
    public interface IJobProductReceiptService
    {
        IEnumerable<JobProductReceipt> SelectLineItems(string jobReceiptId, string userId, string orgId);
        Task<IEnumerable<JobProductReceipt>> SelectAsyncLineItems(string jobReceiptId, string userId, string orgId);
        Task<JobProductReceipt> FindAsync(string id, string userId, string orgId);
        JobProductReceipt Find(string id, string userId, string orgId);
        JobProductReceipt Create(JobProductReceipt param, string userId, string orgId);
        void Update(JobProductReceipt param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<JobProductReceipt> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        Task<IEnumerable<JobProductReceipt>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId);

        decimal TotalQuantityReceipt(string jobNo, string receiptByProcessId, string userId,
            string orgId);

        decimal TotalQuantityReceipt(string jobNo, string receiptByProcessId, string receiptFromProcessId, string userId,
            string orgId);
        
        IEnumerable<JobQuantityTotals> ProcesswiseQuantitiesGroupByJobs(DateTime startDateTime,
            DateTime endDateTime, string receiptByProcessId, string receiptFromProcessId, string userId, string orgId);

        decimal AssemblyLoadIn(DateTime startDateTime, string orgId);

        IEnumerable<JobQuantityTotals> AssemblyLoadInJobwise(DateTime startDateTime, string orgId);
        void Dispose();
    }
}