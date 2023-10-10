using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IProcurementRequestDetailService
    {
        IEnumerable<ProcurementRequestDetail> SelectLineItems(string procurementRequestId, string userId, string orgId);
        Task<IEnumerable<ProcurementRequestDetail>> SelectAsyncLineItems(string procurementRequestId, string userId, string orgId);
        Task<IEnumerable<ProcurementRequestDetail>> SelectAsyncPendingLineItems(string userId, string orgId);
        Task<ProcurementRequestDetail> FindAsync(string id, string userId, string orgId);
        ProcurementRequestDetail Find(string id, string userId, string orgId);
        ProcurementRequestDetail Create(ProcurementRequestDetail param, string userId, string orgId);
        void Update(ProcurementRequestDetail param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<ProcurementRequestDetail> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}