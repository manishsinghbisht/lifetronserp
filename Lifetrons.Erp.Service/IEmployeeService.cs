using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IEmployeeService
    {
        IEnumerable<Employee> Select(string userId, string orgId);
        Task<IEnumerable<Employee>> SelectAsync(string userId, string orgId);
        Task<IEnumerable<Employee>> SelectAsync(string userId, string orgId, List<Guid> containsIds);
        Task<IEnumerable<Employee>> SelectAsyncDepartmentEmployees(string departmentId, string userId, string orgId);
        Task<Employee> FindAsync(string id, string userId, string orgId);
        Employee Find(string id, string userId, string orgId);
        Employee Create(Employee param, string userId, string orgId);
        void Update(Employee param, string userId, string orgId);
        IEnumerable<Employee> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}