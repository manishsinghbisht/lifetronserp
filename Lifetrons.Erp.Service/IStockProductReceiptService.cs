using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IStockProductReceiptService
    {
        IEnumerable<StockProductReceipt> SelectLineItems(string stockReceiptId, string userId, string orgId);
        Task<IEnumerable<StockProductReceipt>> SelectAsyncLineItems(string stockReceiptId, string userId, string orgId);
        Task<IEnumerable<StockProductReceipt>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId);
        Task<StockProductReceipt> FindAsync(string id, string userId, string orgId);
        StockProductReceipt Find(string id, string userId, string orgId);
        StockProductReceipt Create(StockProductReceipt param, string userId, string orgId);
        void Update(StockProductReceipt param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<StockProductReceipt> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
        IEnumerable<StockProductQuantityTotals> CurrentReceiptStatusProductwise(DateTime startDateTime, string orgId);
        IEnumerable<StockProductQuantityTotals> FGCurrentReceiptStatusProductwise(DateTime startDateTime, string orgId);
        IEnumerable<StockProductQuantityTotals> RawCurrentReceiptStatusProductwise(DateTime startDateTime, string orgId);
        IEnumerable<StockProductQuantityTotals> ScrapCurrentReceiptStatusProductwise(DateTime startDateTime, string orgId);
        IEnumerable<StockProductQuantityTotals> ProductReceiptStatus(DateTime startDateTime, string productId, string orgId);
    }
}