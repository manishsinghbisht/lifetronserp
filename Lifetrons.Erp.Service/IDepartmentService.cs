using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> SelectAsync(string userId, string orgId);
        Task<Department> FindAsync(string id, string userId, string orgId);
        Department Find(string id, string userId, string orgId);
        Department Create(Department param, string userId, string orgId);
        void Update(Department param, string userId, string orgId);
        IEnumerable<Department> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Delete(string id);
        void Dispose();
    }
}