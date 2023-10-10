using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IStyleService
    {
        Task<IEnumerable<Style>> GetAsync(string userId, string orgId);
        Task<Style> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}