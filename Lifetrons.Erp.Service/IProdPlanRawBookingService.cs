using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.Service
{
    public interface IProdPlanRawBookingService
    {
        IEnumerable<ProdPlanRawBooking> SelectLineItems(string prodPlanDetailId, string userId, string orgId);
        Task<IEnumerable<ProdPlanRawBooking>> SelectAsyncLineItems(string prodPlanDetailId, string userId, string orgId);

        IEnumerable<ProdPlanRawBooking> SelectLineItems(DateTime startDateTime, DateTime endDateTime,
            string userId, string orgId);

        Task<ProdPlanRawBooking> FindAsync(string id, string userId, string orgId);

        ProdPlanRawBooking Find(string id, string userId, string orgId);

        Boolean Create(string prodPlanDetailId, string jobNo, string userId, string orgId);

        void Delete(string prodPlanDetailId);

        IEnumerable<ProdPlanRawBooking> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
        void Dispose();
    }
}