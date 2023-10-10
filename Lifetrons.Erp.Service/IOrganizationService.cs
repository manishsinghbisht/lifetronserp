using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IOrganizationService
    {
        Task<IEnumerable<Organization>> GetAsync(string userId, string orgId);
        Task<Organization> FindAsync(string id, string userId, string orgId);
        Organization Find(string id, string userId, string orgId);
        Organization Create(Organization param);
        void Update(Organization param);

        Task<IEnumerable<Organization>> GetAsyncOrganizations(string userId, string orgId);

        //void Delete(string id);
        //IEnumerable<Organization> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Dispose();
    }
}