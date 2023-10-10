using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IPriorityService
    {
        Task<IEnumerable<Priority>> GetAsync(string userId, string orgId);
        Task<Priority> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}