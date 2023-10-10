using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IStockItemReceiptService
    {
        IEnumerable<StockItemReceipt> SelectLineItems(string orderId, string userId, string orgId);
        Task<IEnumerable<StockItemReceipt>> SelectAsyncLineItems(string orderId, string userId, string orgId);
        Task<StockItemReceipt> FindAsync(string id, string userId, string orgId);
        StockItemReceipt Find(string id, string userId, string orgId);
        StockItemReceipt Create(StockItemReceipt param, string userId, string orgId);
        void Update(StockItemReceipt param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<StockItemReceipt> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        Task<IEnumerable<StockItemReceipt>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId);
        IEnumerable<StockItemQuantityTotals> ItemReceiptStatus(DateTime startDateTime, string itemId, string orgId);
        IEnumerable<StockItemQuantityTotals> CurrentReceiptStatusItemwise(DateTime startDateTime, string orgId);
        IEnumerable<StockItemQuantityTotals> FGCurrentReceiptStatusItemwise(DateTime startDateTime, string orgId);
        IEnumerable<StockItemQuantityTotals> RawCurrentReceiptStatusItemwise(DateTime startDateTime, string orgId);
        IEnumerable<StockItemQuantityTotals> ScrapCurrentReceiptStatusItemwise(DateTime startDateTime, string orgId);

        void Dispose();
    }
}