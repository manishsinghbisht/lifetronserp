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
    public static class StockProductReceiptRepository
    {

        public static IEnumerable<StockProductQuantityTotals> GetProcesswiseQuantityTotalsGroupByJobs(this IRepository<StockProductReceipt> repository, DateTime startDateTime, DateTime endDateTime, Guid receiptByProcessId, Guid receiptFromProcessId, Guid orgId)
        {

            var receipts = repository.GetRepository<StockProductReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockReceiptHead.Date >= startDateTime
                    & jpi.StockReceiptHead.Date <= endDateTime
                    & jpi.StockReceiptHead.ReceiptFromProcessId == receiptFromProcessId
                    & jpi.StockReceiptHead.ReceiptByProcessId == receiptByProcessId);

            var query = from p in receipts
                        group p by p.ProductId
                            into g
                            select new StockProductQuantityTotals()
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity)
                };

            return query.OrderBy(x => x.ProductId).AsEnumerable();
        }

        public static IEnumerable<StockProductQuantityTotals> ProductReceiptStatus(this IRepository<StockProductReceipt> repository, DateTime startDateTime, Guid productId, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            var receipts = repository.GetRepository<StockProductReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockReceiptHead.Date >= startDateTime
                     & jpi.ProductId.Equals(productId)
                    & jpi.StockReceiptHead.ReceiptByEnterpriseStageId == stockStageGuid);

            var query = from p in receipts
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

        public static IEnumerable<StockProductQuantityTotals> ReceiptStatusProductwise(this IRepository<StockProductReceipt> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            var receipts = repository.GetRepository<StockProductReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockReceiptHead.Date >= startDateTime
                    & jpi.StockReceiptHead.ReceiptByEnterpriseStageId == stockStageGuid);

            var query = from p in receipts
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

        public static IEnumerable<StockProductQuantityTotals> FGReceiptStatusProductwise(this IRepository<StockProductReceipt> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            Guid fgProcessGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["FG Stock"].ToSysGuid();
            var receipts = repository.GetRepository<StockProductReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockReceiptHead.Date >= startDateTime
                    & jpi.StockReceiptHead.ReceiptByEnterpriseStageId == stockStageGuid
                    & jpi.StockReceiptHead.ReceiptByProcessId == fgProcessGuid);

            var query = from p in receipts
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

        public static IEnumerable<StockProductQuantityTotals> RawReceiptStatusProductwise(this IRepository<StockProductReceipt> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            Guid rawProcessGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Raw Stock"].ToSysGuid();
            var receipts = repository.GetRepository<StockProductReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockReceiptHead.Date >= startDateTime
                    & jpi.StockReceiptHead.ReceiptByEnterpriseStageId == stockStageGuid
                    & jpi.StockReceiptHead.ReceiptByProcessId == rawProcessGuid);

            var query = from p in receipts
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

        public static IEnumerable<StockProductQuantityTotals> ScrapReceiptStatusProductwise(this IRepository<StockProductReceipt> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            Guid scrapProcessGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Scrap Stock"].ToSysGuid();
            var receipts = repository.GetRepository<StockProductReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockReceiptHead.Date >= startDateTime
                    & jpi.StockReceiptHead.ReceiptByEnterpriseStageId == stockStageGuid
                    & jpi.StockReceiptHead.ReceiptByProcessId == scrapProcessGuid);

            var query = from p in receipts
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