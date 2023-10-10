using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ICaseStatusService
    {
        Task<IEnumerable<CaseStatu>> GetAsync(string userId, string orgId);
        Task<CaseStatu> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}