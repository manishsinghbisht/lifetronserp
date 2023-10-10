using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IProcurementRequestService
    {
        IEnumerable<ProcurementRequest> Select(string id, string userId, string orgId);
        Task<IEnumerable<ProcurementRequest>> SelectAsync(string userId, string orgId);
        ProcurementRequest Find(string id, string userId, string orgId);
        Task<ProcurementRequest> FindAsync(string id, string userId, string orgId);
        ProcurementRequest Create(ProcurementRequest param, string userId, string orgId);
        void Update(ProcurementRequest param, string userId, string orgId);
        new void Delete(ProcurementRequest model);
        IEnumerable<ProcurementRequest> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}