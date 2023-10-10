using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IStockProductIssueService
    {
        IEnumerable<StockProductIssue> SelectLineItems(string stockIssueId, string userId, string orgId);
        Task<IEnumerable<StockProductIssue>> SelectAsyncLineItems(string stockIssueId, string userId, string orgId);
        Task<IEnumerable<StockProductIssue>> SelectAsyncLineItemsByJobNo(string jobNo, string userId, string orgId);
        Task<StockProductIssue> FindAsync(string id, string userId, string orgId);
        StockProductIssue Find(string id, string userId, string orgId);
        StockProductIssue Create(StockProductIssue param, string userId, string orgId);
        void Update(StockProductIssue param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<StockProductIssue> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
        IEnumerable<StockProductQuantityTotals> CurrentIssueStatusProductwise(DateTime startDateTime, string orgId);
        IEnumerable<StockProductQuantityTotals> FGCurrentIssueStatusProductwise(DateTime startDateTime, string orgId);
        IEnumerable<StockProductQuantityTotals> RawCurrentIssueStatusProductwise(DateTime startDateTime, string orgId);
        IEnumerable<StockProductQuantityTotals> ScrapCurrentIssueStatusProductwise(DateTime startDateTime, string orgId);
        IEnumerable<StockProductQuantityTotals> ProductIssueStatus(DateTime startDateTime, string productId, string orgId);
    }
}