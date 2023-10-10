using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IPriceBookLineItemService
    {
        //Task<IEnumerable<PriceBookLineItem>> GetAsync(string userId, string orgId);
        IEnumerable<PriceBookLineItem> SelectLineItems(string priceBookId, string userId, string orgId);
        Task<IEnumerable<PriceBookLineItem>> SelectAsyncLineItems(string priceBookId, string userId, string orgId);
        PriceBookLineItem Find(string priceBookId, string productId, string userId, string orgId);
        Task<PriceBookLineItem> FindAsync(string priceBookId, string productId, string userId, string orgId);
        PriceBookLineItem Create(PriceBookLineItem param, string userId, string orgId);
        void Update(PriceBookLineItem param, string userId, string orgId);
        IEnumerable<PriceBookLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
        List<PriceBookLineItem> GetDefaultPriceBook(string userId, string orgId);
    }
}