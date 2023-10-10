using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ITargetService
    {
        Task<IEnumerable<Target>> SelectAsync(string userId, string orgId);
        Task<Target> FindAsync(string id, string userId, string orgId);
        Target SelectTarget(string userId, string orgId, DateTime targetDate, string objectId);
        Target Create(Target param, string userId, string orgId);
        void Update(Target param, string userId, string orgId);
        IEnumerable<Target> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}