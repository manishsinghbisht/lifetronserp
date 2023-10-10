using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IWeightUnitService
    {
        Task<IEnumerable<WeightUnit>> SelectAsync(string userId, string orgId);
        Task<WeightUnit> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}