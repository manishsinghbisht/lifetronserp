//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Lifetrons.Erp.Data;

//namespace Lifetrons.Erp.Service
//{
//    public interface IProdPlanService
//    {
//        IEnumerable<ProdPlan> Select(string id, string userId, string orgId);
//        Task<IEnumerable<ProdPlan>> SelectAsync(string userId, string orgId);
//        ProdPlan Find(string id, string userId, string orgId);
//        Task<ProdPlan> FindAsync(string id, string userId, string orgId);
//        ProdPlan Create(ProdPlan param, string userId, string orgId);
//        void Update(ProdPlan param, string userId, string orgId);
//        new void Delete(ProdPlan model);
//        IEnumerable<ProdPlan> GetPagedList(int pageNumber, int pageSize, out int totalRecords, string userId, string orgId);
//        void Dispose();
//    }
//}