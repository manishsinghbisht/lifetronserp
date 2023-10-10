using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ICampaignTypeService
    {
        Task<IEnumerable<CampaignType>> GetAsync(string userId, string orgId);
        Task<CampaignType> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}