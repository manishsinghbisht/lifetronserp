using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IItemCategoryService
    {
        Task<IEnumerable<ItemCategory>> GetAsync(string userId, string orgId);
        Task<ItemCategory> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}