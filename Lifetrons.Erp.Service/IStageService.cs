using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IStageService
    {
        Task<IEnumerable<Stage>> GetAsync(string userId, string orgId);
        Task<Stage> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}