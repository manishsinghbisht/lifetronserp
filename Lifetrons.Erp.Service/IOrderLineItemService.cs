using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IOrderLineItemService
    {
        IEnumerable<OrderLineItem> SelectLineItems(string orderId, string userId, string orgId);
        Task<IEnumerable<OrderLineItem>> SelectAsyncLineItems(string orderId, string userId, string orgId);
        Task<OrderLineItem> FindAsync(string orderId, string priceBookId, string productId, string userId, string orgId);
        Task<OrderLineItem> SelectSingleAsync(string orderLineItemId, string orderId, string userId, string orgId);
        OrderLineItem SelectSingle(string orderLineItemId, string orderId, string userId, string orgId);
        Task<OrderLineItem> SelectSingleAsync(string jobNo, string userId, string orgId);
        OrderLineItem SelectSingle(string jobNo);
        OrderLineItem SelectSingle(string jobNo, string userId, string orgId);
        OrderLineItem Create(OrderLineItem param, string userId, string orgId);
        void Update(OrderLineItem param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<OrderLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}