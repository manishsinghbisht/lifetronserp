using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IOperationBOMLineItemService
    {
        IEnumerable<OperationBOMLineItem> SelectLineItems(string productId, string enterpriseStageId, string processId, string userId, string orgId);
        OperationBOMLineItem Find(string productId, string enterpriseStageId, string processId, string itemId, string userId, string orgId);
        Task<OperationBOMLineItem> FindAsync(string productId, string enterpriseStageId, string processId, string itemId, string userId, string orgId);
        OperationBOMLineItem Create(OperationBOMLineItem param, string userId, string orgId);
        void Update(OperationBOMLineItem param, string userId, string orgId);
        void Delete(OperationBOMLineItem param);
        IEnumerable<OperationBOMLineItem> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}