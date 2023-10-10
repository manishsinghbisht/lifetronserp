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
    public static class JobProductIssueRepository
    {

        public static IEnumerable<JobQuantityTotals> GetProcesswiseQuantityTotalsGroupByJobs(this IRepository<JobProductIssue> repository, DateTime startDateTime, DateTime endDateTime, Guid issuedByProcessId, Guid issuedToProcessId, Guid orgId)
        {

            var productIssues = repository.GetRepository<JobProductIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.JobIssueHead.Date >= startDateTime
                    & jpi.JobIssueHead.Date <= endDateTime
                    & jpi.JobIssueHead.IssuedToProcessId == issuedToProcessId
                    & jpi.JobIssueHead.IssuedByProcessId == issuedByProcessId);

            var query = from p in productIssues
                        group p by p.JobNo
                            into g
                            select new JobQuantityTotals()
                {
                    JobNo = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity)
                };

            return query.OrderBy(x => x.JobNo).AsEnumerable();
        }

        public static IEnumerable<JobQuantityTotals> ProcessLoadInJobwise(this IRepository<JobProductIssue> repository, DateTime startDateTime, Guid issuedToProcessId, Guid orgId)
        {

            var productIssues = repository.GetRepository<JobProductIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.JobIssueHead.Date >= startDateTime
                    & jpi.JobIssueHead.IssuedToProcessId == issuedToProcessId);

            var query = from p in productIssues
                        group p by p.JobNo
                            into g
                            select new JobQuantityTotals()
                            {
                                JobNo = g.Key,
                                TotalQuantity = g.Sum(x => x.Quantity)
                            };

            return query.OrderBy(x => x.JobNo).AsEnumerable();
        }

        public static IEnumerable<JobQuantityTotals> ProcessLoadOutJobwise(this IRepository<JobProductIssue> repository, DateTime startDateTime, Guid issuedByProcessId, Guid orgId)
        {

            var productIssues = repository.GetRepository<JobProductIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.JobIssueHead.Date >= startDateTime
                    & jpi.JobIssueHead.IssuedByProcessId == issuedByProcessId);

            var query = from p in productIssues
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