using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IProcurementOrderDetailService
    {
        IEnumerable<ProcurementOrderDetail> SelectLineItems(string procurementOrderId, string userId, string orgId);
        Task<IEnumerable<ProcurementOrderDetail>> SelectAsyncLineItems(string procurementOrderId, string userId, string orgId);
        Task<ProcurementOrderDetail> FindAsync(string id, string userId, string orgId);
        ProcurementOrderDetail Find(string id, string userId, string orgId);
        ProcurementOrderDetail Create(ProcurementOrderDetail param, string userId, string orgId);
        void Update(ProcurementOrderDetail param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<ProcurementOrderDetail> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
        decimal FindLastProcurementPriceOfItem(string itemId, string orgId);
    }
}