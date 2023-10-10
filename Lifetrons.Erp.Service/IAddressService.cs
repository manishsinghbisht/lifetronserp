using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IAddressService 
    {
        Task<IEnumerable<Address>> SelectAsync(string userId, string orgId);
        Address Find(string id, string userId, string orgId);
        Task<Address> FindAsync(string id, string userId, string orgId);
        Address Create(Address param, string userId, string orgId);
        void Update(Address param, string userId, string orgId);
        IEnumerable<Address> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}