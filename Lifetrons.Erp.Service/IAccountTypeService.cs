using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IAccountTypeService
    {
        Task<IEnumerable<AccountType>> GetAsync(string userId, string orgId);
        Task<AccountType> FindAsync(string id, string userId, string orgId);
        void Dispose();
    }
}