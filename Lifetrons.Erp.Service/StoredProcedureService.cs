using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;
using Repository;

namespace Lifetrons.Erp.Service
{
    public class StoredProcedureService : IStoredProcedureService
    {
        //private readonly IRepository<sViewDashboardOpenWork_Result> _repositoryDashOpenWork;
        //private readonly IRepository<sViewDashboardMonthlyLeadOppComaprison_Result> _repositoryDashMonthlyLeadOppComparison;
        //public StoredProcedureService(IRepository<sViewDashboardOpenWork_Result> repositoryDashOpenWork, IRepository<sViewDashboardMonthlyLeadOppComaprison_Result> repositoryDashMonthlyLeadOppComparison)
        //{
        //    _repositoryDashOpenWork = repositoryDashOpenWork;
        //    _repositoryDashMonthlyLeadOppComparison = repositoryDashMonthlyLeadOppComparison;
        //}

        //public IQueryable<sViewDashboardOpenWork_Result> ExecuteSpViewDashboardOpenWork(string userId, string orgId)
        //{
        //    Guid orgIdGuid = orgId.ToSysGuid();

        //    var ownerIdParameter = userId != null ?
        //        new SqlParameter("@OwnerId", userId) :
        //        new SqlParameter("@OwnerId", typeof(string));

        //    var orgIdParameter = userId != null ?
        //      new SqlParameter("@OrgId", orgIdGuid) :
        //      new SqlParameter("@OrgId", typeof(string));


        //    //var records = _repositoryDashOpenWork
        //    //       .Query()
        //    //       .OrderBy(q => q
        //    //           .OrderBy(c => c.Name));

        //    return _repositoryDashOpenWork.SqlQuery("sViewDashboardOpenWork @OwnerId, @OrgId", new object[] { ownerIdParameter, orgIdParameter });
        //}

        //public IQueryable<sViewDashboardMonthlyLeadOppComaprison_Result> ExecuteSpViewDashboardMonthlyLeadOppComaprison(string userId, string orgId)
        //{
        //    Guid orgIdGuid = orgId.ToSysGuid();

        //    var ownerIdParameter = userId != null ?
        //        new SqlParameter("@OwnerId", userId) :
        //        new SqlParameter("@OwnerId", typeof(string));

        //    var orgIdParameter = userId != null ?
        //      new SqlParameter("@OrgId", orgIdGuid) :
        //      new SqlParameter("@OrgId", typeof(string));


        //    return _repositoryDashMonthlyLeadOppComparison.SqlQuery("sViewDashboardMonthlyLeadOppComaprison  @OwnerId, @OrgId", new object[] { ownerIdParameter, orgIdParameter });
        //}


    }
}
