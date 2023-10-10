using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IProcessTimeConfigService
    {
        Task<IEnumerable<ProcessTimeConfig>> SelectAsync(string userId, string orgId);
        IEnumerable<ProcessTimeConfig> Select(string userId, string orgId);
        IEnumerable<ProcessTimeConfig> Select(string processId, string userId, string orgId);
        Task<ProcessTimeConfig> FindAsync(string id, string userId, string orgId);
        ProcessTimeConfig Create(ProcessTimeConfig param, string userId, string orgId);
        void Update(ProcessTimeConfig param, string userId, string orgId);
        void Dispose();
    }
}