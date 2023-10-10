using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IQuoteService
    {
        Task<IEnumerable<Quote>> GetQuotesByAccountAsync(DateTime startDateTime, DateTime endDateTime, string accountId,
            string orgId);
        IEnumerable<Quote> Select(string quoteId, string userId, string orgId);
        Quote Find(string id, string userId, string orgId);
        Task<IEnumerable<Quote>> SelectAsync(string userId, string orgId);
        Task<Quote> FindAsync(string id, string userId, string orgId);
        Quote Create(Quote param, string userId, string orgId);
        void Update(Quote param, string userId, string orgId);
        IEnumerable<Quote> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}