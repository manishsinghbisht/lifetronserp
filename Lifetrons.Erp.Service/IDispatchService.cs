using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IDispatchService
    {
        IEnumerable<Dispatch> Select(string id, string userId, string orgId);
        Task<IEnumerable<Dispatch>> SelectAsync(string userId, string orgId);
        Dispatch Find(string id, string userId, string orgId);
        Task<Dispatch> FindAsync(string id, string userId, string orgId);
        Dispatch Create(Dispatch param, string userId, string orgId);
        void Update(Dispatch param, string userId, string orgId);
        new void Delete(Dispatch model);
        IEnumerable<Dispatch> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}