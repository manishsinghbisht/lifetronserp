using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IRatingService
    {
        Task<IEnumerable<Rating>> GetAsync(string userId, string orgId);
        Task<Rating> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}