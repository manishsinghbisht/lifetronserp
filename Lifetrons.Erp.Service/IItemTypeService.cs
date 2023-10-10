using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IItemTypeService
    {
        Task<IEnumerable<ItemType>> GetAsync(string userId, string orgId);
        Task<ItemType> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}