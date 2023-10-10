using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IQuoteLineItemService
    {
        IEnumerable<QuoteLineItem> SelectLineItems(string quoteId, string userId, string orgId);
        Task<IEnumerable<QuoteLineItem>> SelectAsyncLineItems(string quoteId, string userId, string orgId);
        Task<QuoteLineItem> FindAsync(string quoteId, string priceBookId, string productId, string userId, string orgId);
        QuoteLineItem Create(QuoteLineItem param, string userId, string orgId);
        void Update(QuoteLineItem param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<QuoteLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}