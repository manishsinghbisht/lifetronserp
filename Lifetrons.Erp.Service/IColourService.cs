using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IColourService
    {
        Task<IEnumerable<Colour>> SelectAsync(string userId, string orgId);
        Task<Colour> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}