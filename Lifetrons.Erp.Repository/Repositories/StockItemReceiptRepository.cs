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
    public static class StockItemReceiptRepository
    {

        public static IEnumerable<StockItemQuantityTotals> GetProcesswiseQuantityTotalsGroupByJobs(this IRepository<StockItemReceipt> repository, DateTime startDateTime, DateTime endDateTime, Guid receiptByProcessId, Guid receiptFromProcessId, Guid orgId)
        {

            var receipts = repository.GetRepository<StockItemReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockReceiptHead.Date >= startDateTime
                    & jpi.StockReceiptHead.Date <= endDateTime
                    & jpi.StockReceiptHead.ReceiptFromProcessId == receiptFromProcessId
                    & jpi.StockReceiptHead.ReceiptByProcessId == receiptByProcessId);

            var query = from p in receipts
                        group p by p.ItemId
                            into g
                            select new StockItemQuantityTotals()
                {
                    ItemId = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity)
                };

            return query.OrderBy(x => x.ItemId).AsEnumerable();
        }

        public static IEnumerable<StockItemQuantityTotals> ItemReceiptStatus(this IRepository<StockItemReceipt> repository, DateTime startDateTime, Guid itemId, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            var receipts = repository.GetRepository<StockItemReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockReceiptHead.Date >= startDateTime
                     & jpi.ItemId.Equals(itemId)
                    & jpi.StockReceiptHead.ReceiptByEnterpriseStageId == stockStageGuid);

            var query = from p in receipts
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

        public static IEnumerable<StockItemQuantityTotals> ReceiptStatusItemwise(this IRepository<StockItemReceipt> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            var receipts = repository.GetRepository<StockItemReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockReceiptHead.Date >= startDateTime
                    & jpi.StockReceiptHead.ReceiptByEnterpriseStageId == stockStageGuid);

            var query = from p in receipts
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

        public static IEnumerable<StockItemQuantityTotals> FGReceiptStatusItemwise(this IRepository<StockItemReceipt> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            Guid fgProcessGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["FG Stock"].ToSysGuid();

            var receipts = repository.GetRepository<StockItemReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockReceiptHead.Date >= startDateTime
                    & jpi.StockReceiptHead.ReceiptByEnterpriseStageId == stockStageGuid
                    & jpi.StockReceiptHead.ReceiptByProcessId == fgProcessGuid);

            var query = from p in receipts
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

        public static IEnumerable<StockItemQuantityTotals> RawReceiptStatusItemwise(this IRepository<StockItemReceipt> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            Guid rawProcessGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Raw Stock"].ToSysGuid();

            var receipts = repository.GetRepository<StockItemReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockReceiptHead.Date >= startDateTime
                    & jpi.StockReceiptHead.ReceiptByEnterpriseStageId == stockStageGuid
                    & jpi.StockReceiptHead.ReceiptByProcessId == rawProcessGuid);

            var query = from p in receipts
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

        public static IEnumerable<StockItemQuantityTotals> ScrapReceiptStatusItemwise(this IRepository<StockItemReceipt> repository, DateTime startDateTime, Guid orgId)
        {
            Guid stockStageGuid = Lifetrons.Erp.Data.Helper.SystemDefinedEnterpriseStages["Stock"].ToSysGuid();
            Guid scrapProcessGuid = Lifetrons.Erp.Data.Helper.SystemDefinedProcesses["Scrap Stock"].ToSysGuid();

            var receipts = repository.GetRepository<StockItemReceipt>().Queryable()
                .Where(jpi => jpi.OrgId == orgId
                    & jpi.StockReceiptHead.Date >= startDateTime
                    & jpi.StockReceiptHead.ReceiptByEnterpriseStageId == stockStageGuid
                    & jpi.StockReceiptHead.ReceiptByProcessId == scrapProcessGuid);

            var query = from p in receipts
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