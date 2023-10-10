using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public interface IBOMService : IService<BOM>
    {
        Task<IEnumerable<BOM>> SelectAsync(string userId, string orgId);
        BOM Find(string productId, string userId, string orgId);
        Task<BOM> FindAsync(string productId, string userId, string orgId);
        BOM Create(BOM param, string userId, string orgId);
        void Update(BOM param, string userId, string orgId);
        IEnumerable<BOM> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Delete(string id);
        void Dispose();
    }
}