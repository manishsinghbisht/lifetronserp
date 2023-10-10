using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ICostingSubGroupService
    {
        Task<IEnumerable<CostingSubGroup>> GetAsync(string userId, string orgId);
        Task<CostingSubGroup> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}