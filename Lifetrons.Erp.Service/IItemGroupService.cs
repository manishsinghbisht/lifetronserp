using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IItemGroupService
    {
        Task<IEnumerable<ItemGroup>> GetAsync(string userId, string orgId);
        Task<ItemGroup> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}