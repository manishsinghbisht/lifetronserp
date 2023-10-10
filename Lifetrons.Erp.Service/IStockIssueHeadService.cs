using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IStockIssueHeadService
    {
        IEnumerable<StockIssueHead> Select(string id, string userId, string orgId);
        Task<IEnumerable<StockIssueHead>> SelectAsync(string userId, string orgId);
        StockIssueHead Find(string id, string userId, string orgId);
        Task<StockIssueHead> FindAsync(string id, string userId, string orgId);
        StockIssueHead Create(StockIssueHead param, string userId, string orgId);
        void Update(StockIssueHead param, string userId, string orgId);
        IEnumerable<StockIssueHead> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Delete(StockIssueHead model);
        void Dispose();
    }
}