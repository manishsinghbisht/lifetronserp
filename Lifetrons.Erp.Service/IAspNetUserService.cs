using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IAspNetUserService
    {
        
        AspNetUser SelectSingleUserWithRoles(string id, string orgId);
        Task<IEnumerable<AspNetUser>> SelectAsync(string orgId);
        Task<AspNetUser> FindAsync(string id);
        AspNetUser Find(string id);
        Task<IEnumerable<AspNetUser>> GetAsyncUsers(string userId, string orgId);
        AspNetUser FindByUserName(string userName);
        // IEnumerable<AspNetUser> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
    }
}