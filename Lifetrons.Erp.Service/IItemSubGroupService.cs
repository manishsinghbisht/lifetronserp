using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IItemSubGroupService
    {
        Task<IEnumerable<ItemSubGroup>> GetAsync(string userId, string orgId);
        Task<ItemSubGroup> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}