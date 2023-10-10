using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IQuoteStatusService
    {
        Task<IEnumerable<QuoteStatu>> GetAsync(string userId, string orgId);
        Task<QuoteStatu> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}