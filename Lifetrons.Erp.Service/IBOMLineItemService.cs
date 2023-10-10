using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IBOMLineItemService
    {
        IEnumerable<BOMLineItem> SelectLineItems(string productId, string userId, string orgId);
        BOMLineItem Find(string productId, string itemId, string userId, string orgId);
        Task<BOMLineItem> FindAsync(string productId, string itemId, string userId, string orgId);
        BOMLineItem Create(BOMLineItem param, string userId, string orgId);
        void Update(BOMLineItem param, string userId, string orgId);
        IEnumerable<BOMLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);

        void Delete(BOMLineItem param);
        void Dispose();
    }
}