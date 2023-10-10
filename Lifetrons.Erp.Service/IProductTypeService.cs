using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IProductTypeService
    {
        Task<IEnumerable<ProductType>> GetAsync(string userId, string orgId);
        Task<ProductType> FindAsync(string id, string userId, string orgId);
        ProductType Create(ProductType param, string userId, string orgId);
        void Update(ProductType param, string userId, string orgId);
        //void Delete(string id);
        IEnumerable<ProductType> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId,
            string orgId);
        void Dispose();
    }
}