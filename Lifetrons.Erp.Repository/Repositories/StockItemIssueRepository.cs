using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Lifetrons.Erp.Data;
using LinqKit;
using Repository.Pattern.Repositories;

namespace Lifetrons.Erp.Repository.Repositories
{
    // Exmaple: How to add custom methods to a repository.
    public static class StockItemIssueRepository
    {

        public static IEnumerable<StockItemQuantityTotals> GetProcesswiseQuantityTotalsGroupByJobs(this IRepository<StockItemIssue> repository, DateTime startDateTime, DateTime endDateTime, Guid issuedByProcessId, Guid issuedToProcessId, Guid orgId)
        {

            var issues = repository.GetRepository<StockItemIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockIssueHead.Date >= startDateTime
                    & jpi.StockIssueHead.Date <= endDateTime
                    & jpi.StockIssueHead.IssuedToProcessId == issuedToProcessId
                    & jpi.StockIssueHead.IssuedByProcessId == issuedByProcessId);

            var query = from p in issues
                        group p by p.ItemId
                            into g
                            select new StockItemQuantityTotals()
                {
                    ItemId = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity)
                };

            return query.OrderBy(x => x.ItemId).AsEnumerable();
        }

        public static IEnumerable<StockItemQuantityTotals> ItemIssueStatus(this IRepository<StockItemIssue> repository, DateTime startDateTime, Guid itemId, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            var productIssues = repository.GetRepository<StockItemIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockIssueHead.Date >= startDateTime
                     & jpi.ItemId.Equals(itemId)
                    & jpi.StockIssueHead.IssuedByEnterpriseStageId == stockStageGuid);

            var query = from p in productIssues
                        orderby p.Item.Name
                        group p by p.ItemId
                            into g
                            select new StockItemQuantityTotals()
                            {
                                ItemId = g.Key,
                                ItemName = g.FirstOrDefault().Item.Name,
                                TotalQuantity = g.Sum(x => x.Quantity)
                            };

            return query.OrderBy(y => y.ItemId).AsEnumerable();
        }

        public static IEnumerable<StockItemQuantityTotals> IssueStatusItemwise(this IRepository<StockItemIssue> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            var productIssues = repository.GetRepository<StockItemIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockIssueHead.Date >= startDateTime
                    & jpi.StockIssueHead.IssuedByEnterpriseStageId == stockStageGuid);

            var query = from p in productIssues
                        orderby p.Item.Name
                        group p by p.ItemId
                            into g
                            select new StockItemQuantityTotals()
                            {
                                ItemId = g.Key,
                                ItemName = g.FirstOrDefault().Item.Name,
                                TotalQuantity = g.Sum(x => x.Quantity)
                            };

            return query.OrderBy(x => x.ItemId).AsEnumerable();
        }

        public static IEnumerable<StockItemQuantityTotals> FGIssueStatusItemwise(this IRepository<StockItemIssue> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            Guid fgProcessGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["FG Stock"].ToSysGuid();

            var productIssues = repository.GetRepository<StockItemIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockIssueHead.Date >= startDateTime
                    & jpi.StockIssueHead.IssuedByEnterpriseStageId == stockStageGuid
                    & jpi.StockIssueHead.IssuedByProcessId == fgProcessGuid);

            var query = from p in productIssues
                        orderby p.Item.Name
                        group p by p.ItemId
                            into g
                        select new StockItemQuantityTotals()
                        {
                            ItemId = g.Key,
                            ItemName = g.FirstOrDefault().Item.Name,
                            TotalQuantity = g.Sum(x => x.Quantity)
                        };

            return query.OrderBy(x => x.ItemId).AsEnumerable();
        }

        public static IEnumerable<StockItemQuantityTotals> RawIssueStatusItemwise(this IRepository<StockItemIssue> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            Guid rawProcessGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Raw Stock"].ToSysGuid();

            var productIssues = repository.GetRepository<StockItemIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockIssueHead.Date >= startDateTime
                    & jpi.StockIssueHead.IssuedByEnterpriseStageId == stockStageGuid
                    & jpi.StockIssueHead.IssuedByProcessId == rawProcessGuid);

            var query = from p in productIssues
                        orderby p.Item.Name
                        group p by p.ItemId
                            into g
                        select new StockItemQuantityTotals()
                        {
                            ItemId = g.Key,
                            ItemName = g.FirstOrDefault().Item.Name,
                            TotalQuantity = g.Sum(x => x.Quantity)
                        };

            return query.OrderBy(x => x.ItemId).AsEnumerable();
        }

        public static IEnumerable<StockItemQuantityTotals> ScrapIssueStatusItemwise(this IRepository<StockItemIssue> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            Guid scrapProcessGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Scrap Stock"].ToSysGuid();

            var productIssues = repository.GetRepository<StockItemIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockIssueHead.Date >= startDateTime
                    & jpi.StockIssueHead.IssuedByEnterpriseStageId == stockStageGuid
                    & jpi.StockIssueHead.IssuedByProcessId == scrapProcessGuid);

            var query = from p in productIssues
                        orderby p.Item.Name
                        group p by p.ItemId
                            into g
                        select new StockItemQuantityTotals()
                        {
                            ItemId = g.Key,
                            ItemName = g.FirstOrDefault().Item.Name,
                            TotalQuantity = g.Sum(x => x.Quantity)
                        };

            return query.OrderBy(x => x.ItemId).AsEnumerable();
        }
    }
}