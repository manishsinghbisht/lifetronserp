using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ICampaignStatuService
    {
        Task<IEnumerable<CampaignStatu>> GetAsync(string userId, string orgId);
        Task<CampaignStatu> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}