using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ILeadSourceService
    {
        Task<IEnumerable<LeadSource>> GetAsync(string userId, string orgId);
        Task<LeadSource> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}