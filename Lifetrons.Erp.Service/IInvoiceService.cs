using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IInvoiceService
    {
        IEnumerable<Invoice> Get(string invoiceId, string userId, string orgId);
        Task<IEnumerable<Invoice>> GetAsync(string userId, string orgId);
        Task<Invoice> FindAsync(string id, string userId, string orgId);
        Invoice Create(Invoice param, string userId, string orgId);
        void Update(Invoice param, string userId, string orgId);
        IEnumerable<Invoice> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}