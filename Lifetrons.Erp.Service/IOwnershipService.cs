using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IOwnershipService
    {
        Task<IEnumerable<Ownership>> GetAsync(string userId, string orgId);
        Task<Ownership> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}