using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IOpportunityLineItemService
    {
        Task<IEnumerable<OpportunityLineItem>> SelectAsyncLineItems(string opportunityId, string userId, string orgId);
        IEnumerable<OpportunityLineItem> SelectLineItems(string opportunityId, string userId, string orgId);
        Task<OpportunityLineItem> FindAsync(string opportunityId, string priceBookId, string productId, string userId, string orgId);
        OpportunityLineItem Create(OpportunityLineItem param, string userId, string orgId);
        void Update(OpportunityLineItem param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<OpportunityLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}