using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IInvoiceLineItemService
    {
        IEnumerable<InvoiceLineItem> SelectLineItems(string invoiceId, string userId, string orgId);
        Task<IEnumerable<InvoiceLineItem>> SelectAsyncLineItems(string invoiceId, string userId, string orgId);
        Task<InvoiceLineItem> FindAsync(string invoiceId, string priceBookId, string productId, string userId, string orgId);
        InvoiceLineItem Create(InvoiceLineItem param, string userId, string orgId);
        void Update(InvoiceLineItem param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<InvoiceLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}