using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IOpportunityService
    {
        Task<IEnumerable<Opportunity>> GetOpportunitiesByAccountAsync(DateTime startDateTime, DateTime endDateTime, string accountId, string orgId);
        Task<IEnumerable<Opportunity>> SelectAsync(string userId, string orgId);
        Opportunity Find(string id, string userId, string orgId);
        Task<Opportunity> FindAsync(string id, string userId, string orgId);
        Opportunity Create(Opportunity param, string userId, string orgId);
        void Update(Opportunity param, string userId, string orgId);
        IEnumerable<Opportunity> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Delete(Opportunity model);
        void Dispose();
    }
}