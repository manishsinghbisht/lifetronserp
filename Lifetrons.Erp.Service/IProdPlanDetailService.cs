using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IProdPlanDetailService
    {
        Task<IEnumerable<ProdPlanDetail>> SelectAsyncLineItems(string jobNo, string userId, string orgId);

        Task<IEnumerable<ProdPlanDetail>> SelectAsyncLineItems(DateTime startDateTime, string userId, string orgId);

        Task<IEnumerable<ProdPlanDetail>> SelectAsyncLineItems(DateTime startDateTime, DateTime endDateTime,
            string userId, string orgId);

        decimal QuantityInPlanning(string jobNo, string userId, string orgId);
        decimal QuantityInProduction(string jobNo, string userId, string orgId);

        bool IssueForProduction(string prodPlanDetailId, string jobNo, string userId, string orgId);

        Task<ProdPlanDetail> FindAsync(string id, string userId, string orgId);
        ProdPlanDetail Find(string id, string userId, string orgId);
        ProdPlanDetail Create(ProdPlanDetail param, string userId, string orgId);
        void Update(ProdPlanDetail param, string userId, string orgId);
        void Delete(string id);
        IEnumerable<ProdPlanDetail> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);

        Task<IEnumerable<ProdPlanDetail>> SelectAsyncActionableItems(string orgId);
        void Dispose();
    }
}