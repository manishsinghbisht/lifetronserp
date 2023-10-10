using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface INoticeBoardService
    {
        Task<IEnumerable<NoticeBoard>> GetAsync(string userId, string orgId);
        IEnumerable<NoticeBoard> Get(string userId, string orgId);
        Task<NoticeBoard> FindAsync(string id, string userId, string orgId);

        NoticeBoard Create(NoticeBoard param, string userId, string orgId);
        void Update(NoticeBoard param, string userId, string orgId);
        IEnumerable<NoticeBoard> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}