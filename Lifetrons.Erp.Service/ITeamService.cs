using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ITeamService
    {
        Task<IEnumerable<Team>> GetAsync(string userId, string orgId, string departmentId = "");
        Team Find(string id, string userId, string orgId);
        Task<Team> FindAsync(string id, string userId, string orgId);
        Team Create(Team param, string userId, string orgId);
        void Update(Team param, string userId, string orgId);
        IEnumerable<Team> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);

        void Delete(string id);
        void Dispose();
    }
}