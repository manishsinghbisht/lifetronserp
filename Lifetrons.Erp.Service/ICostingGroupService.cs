using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ICostingGroupService
    {
        Task<IEnumerable<CostingGroup>> GetAsync(string userId, string orgId);
        Task<CostingGroup> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}