using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IContractService
    {
        Task<IEnumerable<Contract>> SelectAsync(string userId, string orgId);
        Task<Contract> FindAsync(string id, string userId, string orgId);
        Contract Create(Contract param, string userId, string orgId);
        void Update(Contract param, string userId, string orgId);
        IEnumerable<Contract> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}