using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ILevelService
    {
        Task<IEnumerable<Level>> GetAsync(string userId, string orgId);
        Task<Level> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}