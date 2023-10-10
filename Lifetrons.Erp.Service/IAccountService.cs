using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;
using Service.Pattern;

namespace Lifetrons.Erp.Service
{
    public interface IAccountService : IService<Account>
    {
        Task<IEnumerable<Account>> SelectAsync(string userId, string orgId);
        Account Find(string id, string userId, string orgId);
        Task<Account> FindAsync(string id, string userId, string orgId);
        Account Create(Account param, string userId, string orgId);
        void Update(Account param, string userId, string orgId);
        IEnumerable<Account> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);

        IEnumerable<Account> AccountsWithQuantitiesMorethan(decimal quantity);
        IEnumerable<Account> AccountsWithPurchasesMorethan(decimal amount);
        void Dispose();
    }
}