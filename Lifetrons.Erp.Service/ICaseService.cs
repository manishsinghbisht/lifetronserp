using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ICaseService
    {
        Task<IEnumerable<Case>> SelectAsyncCasesByAccount(DateTime startDateTime, DateTime endDateTime, string accountId, string orgId);
        Task<IEnumerable<Case>> SelectAsync(string userId, string orgId);
        Task<IEnumerable<Case>> SelectAsyncAllCases(string userId, string orgId);
        Task<Case> FindAsync(string id, string userId, string orgId);
        Case Find(string id, string userId, string orgId);
        Case Create(Case param, string userId, string orgId);
        void Update(Case param, string userId, string orgId);
        IEnumerable<Case> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}