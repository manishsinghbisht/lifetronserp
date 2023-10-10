using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IJobItemReceiptService
    {
        IEnumerable<JobItemReceipt> SelectLineItems(string jobReceiptId, string userId, string orgId);
        Task<IEnumerable<JobItemReceipt>> SelectAsyncLineItems(string jobReceiptId, string userId, string orgId);
        Task<JobItemReceipt> FindAsync(string id, string userId, string orgId);
        JobItemReceipt Find(string id, string userId, string orgId);
        JobItemReceipt Create(JobItemReceipt param, string userId, string orgId);
        void Update(JobItemReceipt param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<JobItemReceipt> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        Task<IEnumerable<JobItemReceipt>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId);
        void Dispose();
    }
}