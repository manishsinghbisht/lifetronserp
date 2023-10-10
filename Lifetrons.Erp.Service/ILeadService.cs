using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface ILeadService
    {
        Task<IEnumerable<Lead>> SelectAsync(string userId, string orgId);
        Task<Lead> FindAsync(string id, string userId, string orgId);
        Lead Create(Lead param, string userId, string orgId);
        void Update(Lead param, string userId, string orgId);
        IEnumerable<Lead> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}