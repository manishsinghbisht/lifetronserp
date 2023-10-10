using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IInvoiceStatusService
    {
        Task<IEnumerable<InvoiceStatu>> GetAsync(string userId, string orgId);
        Task<InvoiceStatu> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}