using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ICaseReasonService
    {
        Task<IEnumerable<CaseReason>> GetAsync(string userId, string orgId);
        Task<CaseReason> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}