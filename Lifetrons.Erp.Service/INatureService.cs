using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface INatureService
    {
        Task<IEnumerable<Nature>> GetAsync(string userId, string orgId);
        Task<Nature> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}