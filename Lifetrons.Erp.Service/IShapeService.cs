using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IShapeService
    {
        Task<IEnumerable<Shape>> GetAsync(string userId, string orgId);
        Task<Shape> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}