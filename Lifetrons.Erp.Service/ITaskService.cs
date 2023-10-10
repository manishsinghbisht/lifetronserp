using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifetrons.Erp.Service
{
    public interface ITaskService
    {
        System.Threading.Tasks.Task<IEnumerable<Lifetrons.Erp.Data.Task>> SelectAsyncTasksByAccount(DateTime startDateTime,
            DateTime endDateTime, string accountId, string orgId);
        IEnumerable<Lifetrons.Erp.Data.Task> Select(DateTime startDateTime, DateTime endDateTime, string userId, string orgId);
        Task<IEnumerable<Lifetrons.Erp.Data.Task>> SelectAsync(string userId, string orgId);
        Task<Lifetrons.Erp.Data.Task> FindAsync(string id, string userId, string orgId);
        Lifetrons.Erp.Data.Task Create(Lifetrons.Erp.Data.Task param, string userId, string orgId);
        void Update(Lifetrons.Erp.Data.Task param, string userId, string orgId);
        IEnumerable<Lifetrons.Erp.Data.Task> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);

        string GetRelatedToIdName(string relatedToObjectName, string relatedToId, string userId, string orgId);

        void Delete(string id);
        void Dispose();
    }
}