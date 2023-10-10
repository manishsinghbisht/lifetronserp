using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ILeadStatuService
    {
        Task<IEnumerable<LeadStatu>> GetAsync(string userId, string orgId);
        Task<LeadStatu> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}