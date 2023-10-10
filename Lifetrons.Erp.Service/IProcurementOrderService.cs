using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IProcurementOrderService
    {
        IEnumerable<ProcurementOrder> Select(string id, string userId, string orgId);
        Task<IEnumerable<ProcurementOrder>> SelectAsync(string userId, string orgId);
        ProcurementOrder Find(string id, string userId, string orgId);
        Task<ProcurementOrder> FindAsync(string id, string userId, string orgId);
        ProcurementOrder Create(ProcurementOrder param, string userId, string orgId);
        void Update(ProcurementOrder param, string userId, string orgId);
        new void Delete(ProcurementOrder model);
        IEnumerable<ProcurementOrder> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}