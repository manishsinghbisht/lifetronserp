using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IOperationService
    {
        Task<IEnumerable<Operation>> SelectLineItemsAsync(string productId, string userId, string orgId);

        Task<Operation> FindAsync(string productId, string enterpriseStageId, string processId, string userId,
            string orgId);
        Operation Create(Operation param, string userId, string orgId);
        void Update(Operation param, string userId, string orgId);
        void Delete(Operation param);
        void Dispose();
    }
}