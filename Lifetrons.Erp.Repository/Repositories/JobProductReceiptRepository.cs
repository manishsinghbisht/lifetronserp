using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Repository.Models;
using LinqKit;
using Repository.Pattern.Repositories;

namespace Lifetrons.Erp.Repository.Repositories
{
    // Exmaple: How to add custom methods to a repository.
    public static class JobProductReceiptRepository
    {

        public static IEnumerable<JobQuantityTotals> GetProcesswiseQuantitiesGroupByJobs(this IRepository<JobProductReceipt> repository, DateTime startDateTime, DateTime endDateTime, Guid receiptByProcessId, Guid receiptFromProcessId, Guid orgId)
        {

            var productReceipt = repository.GetRepository<JobProductReceipt>().Queryable()
                .Where(jpr => jpr.OrgId == orgId
                    & jpr.JobReceiptHead.Date >= startDateTime
                    & jpr.JobReceiptHead.Date <= endDateTime
                    & jpr.JobReceiptHead.ReceiptByProcessId == receiptByProcessId
                    & jpr.JobReceiptHead.ReceiptFromProcessId == receiptFromProcessId);

            var query = from p in productReceipt
                group p by p.JobNo
                into g
                            select new JobQuantityTotals()
                {
                    JobNo = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity)
                };
                
            return query.OrderBy(x => x.JobNo).AsEnumerable();
        }

        public static IEnumerable<JobQuantityTotals> AssemblyLoadInJobwise(this IRepository<JobProductReceipt> repository, DateTime startDateTime, Guid orgId)
        {
            Guid receiptByProcessIdGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Assembly"].ToSysGuid();
            var productReceipts = repository.GetRepository<JobProductReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.JobReceiptHead.Date >= startDateTime
                    & jpi.JobReceiptHead.ReceiptByProcessId == receiptByProcessIdGuid);

            var query = from p in productReceipts
                        group p by p.JobNo
                            into g
                            select new JobQuantityTotals()
                            {
                                JobNo = g.Key,
                                TotalQuantity = g.Sum(x => x.Quantity)
                            };

            return query.OrderBy(x => x.JobNo).AsEnumerable();
        }
    }
}