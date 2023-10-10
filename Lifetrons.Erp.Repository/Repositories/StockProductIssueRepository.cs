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
    public static class StockProductIssueRepository
    {

        public static IEnumerable<StockProductQuantityTotals> GetProcesswiseQuantityTotalsGroupByJobs(this IRepository<StockProductIssue> repository, DateTime startDateTime, DateTime endDateTime, Guid issuedByProcessId, Guid issuedToProcessId, Guid orgId)
        {

            var issues = repository.GetRepository<StockProductIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockIssueHead.Date >= startDateTime
                    & jpi.StockIssueHead.Date <= endDateTime
                    & jpi.StockIssueHead.IssuedToProcessId == issuedToProcessId
                    & jpi.StockIssueHead.IssuedByProcessId == issuedByProcessId);

            var query = from p in issues
                        group p by p.ProductId
                            into g
                            select new StockProductQuantityTotals()
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity)
                };

            return query.OrderBy(x => x.ProductId).AsEnumerable();
        }

        public static IEnumerable<StockProductQuantityTotals> ProductIssueStatus(this IRepository<StockProductIssue> repository, DateTime startDateTime, Guid productId, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            var productIssues = repository.GetRepository<StockProductIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockIssueHead.Date >= startDateTime
                     & jpi.ProductId.Equals(productId)
                    & jpi.StockIssueHead.IssuedByEnterpriseStageId == stockStageGuid);

            var query = from p in productIssues
                        orderby p.Product.Name
                        group p by p.ProductId
                            into g
                            select new StockProductQuantityTotals()
                            {
                                ProductId = g.Key,
                                ProductName = g.FirstOrDefault().Product.Name,
                                TotalQuantity = g.Sum(x => x.Quantity)
                            };

            return query.OrderBy(y => y.ProductId).AsEnumerable();
        }

        public static IEnumerable<StockProductQuantityTotals> IssueStatusProductwise(this IRepository<StockProductIssue> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            var productIssues = repository.GetRepository<StockProductIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockIssueHead.Date >= startDateTime
                    & jpi.StockIssueHead.IssuedByEnterpriseStageId == stockStageGuid);

            var query = from p in productIssues
                        orderby p.Product.Name
                        group p by p.ProductId
                            into g
                            select new StockProductQuantityTotals()
                            {
                                ProductId = g.Key,
                                ProductName = g.FirstOrDefault().Product.Name,
                                TotalQuantity = g.Sum(x => x.Quantity)
                            };

            return query.OrderBy(x => x.ProductId).AsEnumerable();
        }

        public static IEnumerable<StockProductQuantityTotals> FGIssueStatusProductwise(this IRepository<StockProductIssue> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            Guid fgProcessGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["FG Stock"].ToSysGuid();

            var productIssues = repository.GetRepository<StockProductIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockIssueHead.Date >= startDateTime
                    & jpi.StockIssueHead.IssuedByEnterpriseStageId == stockStageGuid
                    & jpi.StockIssueHead.IssuedByProcessId == fgProcessGuid);

            var query = from p in productIssues
                        orderby p.Product.Name
                        group p by p.ProductId
                            into g
                        select new StockProductQuantityTotals()
                        {
                            ProductId = g.Key,
                            ProductName = g.FirstOrDefault().Product.Name,
                            TotalQuantity = g.Sum(x => x.Quantity)
                        };

            return query.OrderBy(x => x.ProductId).AsEnumerable();
        }

        public static IEnumerable<StockProductQuantityTotals> RawIssueStatusProductwise(this IRepository<StockProductIssue> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            Guid rawProcessGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Raw Stock"].ToSysGuid();

            var productIssues = repository.GetRepository<StockProductIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockIssueHead.Date >= startDateTime
                    & jpi.StockIssueHead.IssuedByEnterpriseStageId == stockStageGuid
                    & jpi.StockIssueHead.IssuedByProcessId == rawProcessGuid);

            var query = from p in productIssues
                        orderby p.Product.Name
                        group p by p.ProductId
                            into g
                        select new StockProductQuantityTotals()
                        {
                            ProductId = g.Key,
                            ProductName = g.FirstOrDefault().Product.Name,
                            TotalQuantity = g.Sum(x => x.Quantity)
                        };

            return query.OrderBy(x => x.ProductId).AsEnumerable();
        }

        public static IEnumerable<StockProductQuantityTotals> ScrapIssueStatusProductwise(this IRepository<StockProductIssue> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            Guid scrapProcessGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Scrap Stock"].ToSysGuid();

            var productIssues = repository.GetRepository<StockProductIssue>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockIssueHead.Date >= startDateTime
                    & jpi.StockIssueHead.IssuedByEnterpriseStageId == stockStageGuid
                    & jpi.StockIssueHead.IssuedByProcessId == scrapProcessGuid);

            var query = from p in productIssues
                        orderby p.Product.Name
                        group p by p.ProductId
                            into g
                        select new StockProductQuantityTotals()
                        {
                            ProductId = g.Key,
                            ProductName = g.FirstOrDefault().Product.Name,
                            TotalQuantity = g.Sum(x => x.Quantity)
                        };

            return query.OrderBy(x => x.ProductId).AsEnumerable();
        }
    }
}