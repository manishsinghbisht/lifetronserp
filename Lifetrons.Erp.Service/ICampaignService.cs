using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ICampaignService
    {
        Task<IEnumerable<Campaign>> SelectAsync(string userId, string orgId);
        Campaign Find(string id, string userId, string orgId);
        Task<Campaign> FindAsync(string id, string userId, string orgId);
        Campaign Create(Campaign param, string userId, string orgId);
        void Update(Campaign param, string userId, string orgId);
        IEnumerable<Campaign> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}