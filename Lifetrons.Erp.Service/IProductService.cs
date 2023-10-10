using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IProductService
    {
        IEnumerable<Product> Select(string userId, string orgId);
        Task<IEnumerable<Product>> SelectAsync(string userId, string orgId);
        Task<Product> FindAsync(string id, string userId, string orgId);
        Product Find(string id, string userId, string orgId);
        Product Create(Product param, string userId, string orgId);
        void Update(Product param, string userId, string orgId);
        //void Delete(string id);
        IEnumerable<Product> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}