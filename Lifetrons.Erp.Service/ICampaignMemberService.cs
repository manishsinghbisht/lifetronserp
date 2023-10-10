using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ICampaignMemberService
    {
        IEnumerable<CampaignMember> SelectLineItems(string campaignId, string userId, string orgId);
        Task<IEnumerable<CampaignMember>> SelectAsyncLineItems(string campaignId, string userId, string orgId);
        Task<CampaignMember> FindAsync(string id, string userId, string orgId);
        CampaignMember Create(CampaignMember param, string userId, string orgId);
        void Update(CampaignMember param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<CampaignMember> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}