using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IProductFamilyService
    {
        Task<IEnumerable<ProductFamily>> GetAsync(string userId, string orgId);
        Task<ProductFamily> FindAsync(string id, string userId, string orgId);
        ProductFamily Create(ProductFamily param, string userId, string orgId);
        void Update(ProductFamily param, string userId, string orgId);
        //void Delete(string id);
        IEnumerable<ProductFamily> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}