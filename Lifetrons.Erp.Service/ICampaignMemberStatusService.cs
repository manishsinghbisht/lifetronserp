using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ICampaignMemberStatusService
    {
        IEnumerable<CampaignMemberStatu> SelectLineItems(string userId, string orgId);
        Task<IEnumerable<CampaignMemberStatu>> SelectAsyncLineItems(string userId, string orgId);
        Task<CampaignMemberStatu> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}