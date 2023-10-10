using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IPriceBookService
    {
        IEnumerable<PriceBook> Select(string userId, string orgId);
        Task<IEnumerable<PriceBook>> SelectAsync(string userId, string orgId);
        PriceBook Find(string id, string userId, string orgId);
        Task<PriceBook> FindAsync(string id, string userId, string orgId);
        PriceBook Create(PriceBook param, string userId, string orgId);
        void Update(PriceBook param, string userId, string orgId);
        IEnumerable<PriceBook> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}