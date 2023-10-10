using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IItemClassificationService
    {
        Task<IEnumerable<ItemClassification>> GetAsync(string userId, string orgId);
        Task<ItemClassification> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}