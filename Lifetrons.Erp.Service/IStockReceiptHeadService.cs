using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IStockReceiptHeadService
    {
        IEnumerable<StockReceiptHead> Select(string id, string userId, string orgId);
        Task<IEnumerable<StockReceiptHead>> SelectAsync(string userId, string orgId);
        StockReceiptHead Find(string id, string userId, string orgId);
        Task<StockReceiptHead> FindAsync(string id, string userId, string orgId);
        StockReceiptHead Create(StockReceiptHead param, string userId, string orgId);
        void Update(StockReceiptHead param, string userId, string orgId);
        IEnumerable<StockReceiptHead> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Delete(StockReceiptHead model);
        void Dispose();
    }
}