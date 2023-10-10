using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IDeliveryStatuService
    {
        Task<IEnumerable<DeliveryStatu>> GetAsync(string userId, string orgId);
        Task<DeliveryStatu> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}