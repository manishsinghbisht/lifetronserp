using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IJoiningRequestService
    {
        Task<IEnumerable<JoiningRequest>> GetAsync(string userId, string orgId);
        Task<JoiningRequest> FindAsync(string id);
        Task<IEnumerable<JoiningRequest>> FindAsyncEnumerable(string id);
        JoiningRequest Create(JoiningRequest param);
        void Update(JoiningRequest param);
        //void Delete(string id);
        IEnumerable<JoiningRequest> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Dispose();
    }
}