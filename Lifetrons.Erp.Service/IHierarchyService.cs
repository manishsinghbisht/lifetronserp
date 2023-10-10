using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IHierarchyService
    {
        IEnumerable<Hierarchy> SelectDepartmentHierarchy(string userId, string orgId, string departmentId);
        IEnumerable<Hierarchy> SelectTeamHierarchy(string userId, string orgId, string teamId);
        IEnumerable<Hierarchy> SelectDownlineHierarchy(string userId, string orgId);
        IEnumerable<Hierarchy> SelectUserHierarchy(string userId, string orgId);
        IEnumerable<Hierarchy> Select(string userId, string orgId);
        Task<IEnumerable<Hierarchy>> SelectAsync(string userId, string orgId);
        Task<Hierarchy> FindAsync(string id, string userId, string orgId);
        Hierarchy Create(Hierarchy param, string userId, string orgId);
        void Update(Hierarchy param, string userId, string orgId);
        void Delete(string id);
        void Dispose();
    }
}