using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IStockItemIssueService
    {
        IEnumerable<StockItemIssue> SelectLineItems(string stockIssueId, string userId, string orgId);
        Task<IEnumerable<StockItemIssue>> SelectAsyncLineItems(string orderId, string userId, string orgId);
        Task<StockItemIssue> FindAsync(string id, string userId, string orgId);
        StockItemIssue Find(string id, string userId, string orgId);
        StockItemIssue Create(StockItemIssue param, string userId, string orgId);
        void Update(StockItemIssue param, string userId, string orgId);
        Task<IEnumerable<StockItemIssue>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId);
        IEnumerable<StockItemQuantityTotals> CurrentIssueStatusItemwise(DateTime startDateTime, string orgId);
        IEnumerable<StockItemQuantityTotals> FGCurrentIssueStatusItemwise(DateTime startDateTime, string orgId);
        IEnumerable<StockItemQuantityTotals> RawCurrentIssueStatusItemwise(DateTime startDateTime, string orgId);
        IEnumerable<StockItemQuantityTotals> ScrapCurrentIssueStatusItemwise(DateTime startDateTime, string orgId);
        IEnumerable<StockItemQuantityTotals> ItemIssueStatus(DateTime startDateTime, string itemId, string orgId);

        void Delete(string id);
    }
}