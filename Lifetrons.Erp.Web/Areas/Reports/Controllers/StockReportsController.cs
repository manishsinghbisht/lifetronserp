using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;

namespace Lifetrons.Erp.Reports.Controllers
{

    public class StockReportsController : BaseController
    {
        [Dependency]
        public IProcessService ProcessService { get; set; }

        [Dependency]
        public IProdPlanDetailService ProdPlanDetailService { get; set; }

        [Dependency]
        public IJobIssueHeadService JobIssueHeadService { get; set; }

        [Dependency]
        public IJobReceiptHeadService JobReceiptHeadService { get; set; }

        [Dependency]
        public IJobProductIssueService JobProductIssueService { get; set; }

        [Dependency]
        public IJobProductReceiptService JobProductReceiptService { get; set; }

        [Dependency]
        public IStockItemIssueService StockItemIssueService { get; set; }

        [Dependency]
        public IStockItemReceiptService StockItemReceiptService { get; set; }

        [Dependency]
        public IStockProductIssueService StockProductIssueService { get; set; }

        [Dependency]
        public IStockProductReceiptService StockProductReceiptService { get; set; }

        [Dependency]
        public IItemService ItemService { get; set; }

        [Dependency]
        public IProductService ProductService { get; set; }

        [Dependency]
        public IDispatchService DispatchService { get; set; }

        [Dependency]
        public IOrderLineItemService OrderLineItemService { get; set; }

        [Dependency]
        public IProcurementOrderDetailService ProcurementOrderDetailService { get; set; }

        [Dependency]
        public IPriceBookLineItemService PriceBookLineItemService { get; set; }


        #region Product Stock

        [HttpGet]
        public async Task<ActionResult> ProductStockStatus()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.ProductId = new SelectList(await ProductService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ProductStockStatus(string startDate, string productId)
        {
            DateTime startDateTime = Convert.ToDateTime(startDate);
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var viewModelList = new List<StockProductQuantityTotals>();

            var products = await ProductService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());

            IEnumerable<StockProductQuantityTotals> loadIn;
            IEnumerable<StockProductQuantityTotals> loadOut;
            List<StockProductQuantityTotals> loadOutList = new List<StockProductQuantityTotals>();

            loadIn = StockProductReceiptService.ProductReceiptStatus(startDateTime, productId, applicationUser.OrgId.ToString());
            loadOut = StockProductIssueService.ProductIssueStatus(startDateTime, productId, applicationUser.OrgId.ToString());

            if (loadOut.Any())
            {
                loadOutList = loadOut.ToList();
            }

            //Get default price Book List
            var defaultPriceBook = PriceBookLineItemService.GetDefaultPriceBook(applicationUser.Id, applicationUser.OrgId.ToString());

            foreach (var ins in loadIn)
            {
                //Initialze and set rcpts qty to zero if no rcpts found
                var outs = loadOutList.Find(r => r.ProductId == ins.ProductId) ??
                           new StockProductQuantityTotals() { ProductId = ins.ProductId, ProductName = ins.ProductName, TotalQuantity = 0 };

                var viewModel = new StockProductQuantityTotals();
                viewModel.ProductName = ins.ProductName;
                viewModel.ProductId = ins.ProductId;
                viewModel.TotalQuantity = ins.TotalQuantity - outs.TotalQuantity;
                decimal price = 0;
                var priceBookLineItem = defaultPriceBook.Find(p => p.ProductId == viewModel.ProductId);
                if (priceBookLineItem != null)
                {
                    price = priceBookLineItem.ListPrice;
                }
                viewModel.Value = price * viewModel.TotalQuantity;
                viewModelList.Add(viewModel);

            }


            return View(viewModelList);
        }

        #endregion Product Stock

        #region Item Stock

        [HttpGet]
        public async Task<ActionResult> ItemStockStatus()
        {
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            ViewBag.ItemId = new SelectList(await ItemService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ItemStockStatus(string startDate, string itemId)
        {
            DateTime startDateTime = Convert.ToDateTime(startDate);
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var viewModelList = new List<StockItemQuantityTotals>();

            var items = await ItemService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString());

            IEnumerable<StockItemQuantityTotals> loadIn;
            IEnumerable<StockItemQuantityTotals> loadOut;
            List<StockItemQuantityTotals> loadOutList = new List<StockItemQuantityTotals>();

            loadIn = StockItemReceiptService.ItemReceiptStatus(startDateTime, itemId, applicationUser.OrgId.ToString());
            loadOut = StockItemIssueService.ItemIssueStatus(startDateTime, itemId, applicationUser.OrgId.ToString());

            if (loadOut.Any())
            {
                loadOutList = loadOut.ToList();
            }
            foreach (var ins in loadIn)
            {
                //Initialze and set rcpts qty to zero if no rcpts found
                var outs = loadOutList.Find(r => r.ItemId == ins.ItemId) ??
                           new StockItemQuantityTotals() { ItemId = ins.ItemId, ItemName = ins.ItemName, TotalQuantity = 0 };

                var viewModel = new StockItemQuantityTotals();
                viewModel.ItemName = ins.ItemName;
                viewModel.ItemId = ins.ItemId;
                viewModel.TotalQuantity = ins.TotalQuantity - outs.TotalQuantity;
                viewModel.Value = viewModel.TotalQuantity * ProcurementOrderDetailService.FindLastProcurementPriceOfItem(viewModel.ItemId.ToString(), applicationUser.OrgId.ToString());
                viewModelList.Add(viewModel);

            }


            return View(viewModelList);
        }

        #endregion Item Stock

        #region Current Stock

        [HttpGet]
        public async Task<ActionResult> CurrentStockStatus()
        {
            return View();
        }

        public async Task<ActionResult> CurrentStockStatus(string startDate)
        {
            List<StockItemQuantityTotals> viewModelList = CurrentStockForItems(startDate);
            List<StockProductQuantityTotals> viewModelListProducts = CurrentStockForProducts(startDate);

            foreach (var product in viewModelListProducts)
            {
                StockItemQuantityTotals model = new StockItemQuantityTotals();
                model.ItemName = product.ProductName;
                model.TotalQuantity = product.TotalQuantity;
                model.Value = product.Value;
                viewModelList.Add(model);
            }

            return View(viewModelList);
        }

        [HttpGet]
        public async Task<ActionResult> FGCurrentStockStatus()
        {
            return View();
        }

        public async Task<ActionResult> FGCurrentStockStatus(string startDate)
        {
            List<StockItemQuantityTotals> viewModelList = CurrentStockForItems(startDate, "FG Stock");
            List<StockProductQuantityTotals> viewModelListProducts = CurrentStockForProducts(startDate, "FG Stock");

            foreach (var product in viewModelListProducts)
            {
                StockItemQuantityTotals model = new StockItemQuantityTotals();
                model.ItemName = product.ProductName;
                model.TotalQuantity = product.TotalQuantity;
                model.Value = product.Value;
                viewModelList.Add(model);
            }
            ViewBag.Process = "FG Stock";
            return View(viewModelList);
        }

        [HttpGet]
        public async Task<ActionResult> RawCurrentStockStatus()
        {
            return View();
        }

        public async Task<ActionResult> RawCurrentStockStatus(string startDate)
        {
            List<StockItemQuantityTotals> viewModelList = CurrentStockForItems(startDate, "Raw Stock");
            List<StockProductQuantityTotals> viewModelListProducts = CurrentStockForProducts(startDate, "Raw Stock");

            foreach (var product in viewModelListProducts)
            {
                StockItemQuantityTotals model = new StockItemQuantityTotals();
                model.ItemName = product.ProductName;
                model.TotalQuantity = product.TotalQuantity;
                model.Value = product.Value;
                viewModelList.Add(model);
            }

            ViewBag.Process = "Raw Stock";
            return View(viewModelList);
        }

        [HttpGet]
        public async Task<ActionResult> ScrapCurrentStockStatus()
        {
            return View();
        }

        public async Task<ActionResult> ScrapCurrentStockStatus(string startDate)
        {
            List<StockItemQuantityTotals> viewModelList = CurrentStockForItems(startDate, "Scrap Stock");
            List<StockProductQuantityTotals> viewModelListProducts = CurrentStockForProducts(startDate, "Scrap Stock");

            foreach (var product in viewModelListProducts)
            {
                StockItemQuantityTotals model = new StockItemQuantityTotals();
                model.ItemName = product.ProductName;
                model.TotalQuantity = product.TotalQuantity;
                model.Value = product.Value;
                viewModelList.Add(model);
            }

            ViewBag.Process = "Scrap Stock";
            return View(viewModelList);
        }

        private List<StockItemQuantityTotals> CurrentStockForItems(string startDate, string process = "")
        {
            DateTime startDateTime = Convert.ToDateTime(startDate);
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var viewModelList = new List<StockItemQuantityTotals>();

            IEnumerable<StockItemQuantityTotals> loadIn;
            IEnumerable<StockItemQuantityTotals> loadOut;
            List<StockItemQuantityTotals> loadOutList = new List<StockItemQuantityTotals>();

            if (process == "FG Stock")
            {
                loadIn = StockItemReceiptService.FGCurrentReceiptStatusItemwise(startDateTime, applicationUser.OrgId.ToString());
                loadOut = StockItemIssueService.FGCurrentIssueStatusItemwise(startDateTime, applicationUser.OrgId.ToString());
            }
            else if (process == "Raw Stock")
            {
                loadIn = StockItemReceiptService.RawCurrentReceiptStatusItemwise(startDateTime, applicationUser.OrgId.ToString());
                loadOut = StockItemIssueService.RawCurrentIssueStatusItemwise(startDateTime, applicationUser.OrgId.ToString());
            }
            else if (process == "Scrap Stock")
            {
                loadIn = StockItemReceiptService.ScrapCurrentReceiptStatusItemwise(startDateTime, applicationUser.OrgId.ToString());
                loadOut = StockItemIssueService.ScrapCurrentIssueStatusItemwise(startDateTime, applicationUser.OrgId.ToString());
            }
            else
            {
                loadIn = StockItemReceiptService.CurrentReceiptStatusItemwise(startDateTime, applicationUser.OrgId.ToString());
                loadOut = StockItemIssueService.CurrentIssueStatusItemwise(startDateTime, applicationUser.OrgId.ToString());
            }

            if (loadOut.Any())
            {
                loadOutList = loadOut.ToList();
            }

            foreach (var ins in loadIn)
            {
                //Initialze and set rcpts qty to zero if no rcpts found
                var outs = loadOutList.Find(r => r.ItemId == ins.ItemId) ??
                           new StockItemQuantityTotals()
                           {
                               ItemId = ins.ItemId,
                               ItemName = ins.ItemName,
                               TotalQuantity = 0,
                               Value = 0
                           };

                var viewModel = new StockItemQuantityTotals();
                viewModel.ItemName = ins.ItemName;
                viewModel.ItemId = ins.ItemId;
                viewModel.TotalQuantity = ins.TotalQuantity - outs.TotalQuantity;
                viewModel.Value = viewModel.TotalQuantity * ProcurementOrderDetailService.FindLastProcurementPriceOfItem(viewModel.ItemId.ToString(), applicationUser.OrgId.ToString());
                viewModelList.Add(viewModel);
            }

            return viewModelList;
        }

        private List<StockProductQuantityTotals> CurrentStockForProducts(string startDate, string process = "")
        {
            DateTime startDateTime = Convert.ToDateTime(startDate);
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            var viewModelList = new List<StockProductQuantityTotals>();

            IEnumerable<StockProductQuantityTotals> loadIn;
            IEnumerable<StockProductQuantityTotals> loadOut;
            List<StockProductQuantityTotals> loadOutList = new List<StockProductQuantityTotals>();

            if (process == "FG Stock")
            {
                loadIn = StockProductReceiptService.FGCurrentReceiptStatusProductwise(startDateTime, applicationUser.OrgId.ToString());
                loadOut = StockProductIssueService.FGCurrentIssueStatusProductwise(startDateTime, applicationUser.OrgId.ToString());
            }
            else if (process == "Raw Stock")
            {
                loadIn = StockProductReceiptService.RawCurrentReceiptStatusProductwise(startDateTime, applicationUser.OrgId.ToString());
                loadOut = StockProductIssueService.RawCurrentIssueStatusProductwise(startDateTime, applicationUser.OrgId.ToString());
            }
            else if (process == "Scrap Stock")
            {
                loadIn = StockProductReceiptService.ScrapCurrentReceiptStatusProductwise(startDateTime, applicationUser.OrgId.ToString());
                loadOut = StockProductIssueService.ScrapCurrentIssueStatusProductwise(startDateTime, applicationUser.OrgId.ToString());
            }
            else
            {
                loadIn = StockProductReceiptService.CurrentReceiptStatusProductwise(startDateTime, applicationUser.OrgId.ToString());
                loadOut = StockProductIssueService.CurrentIssueStatusProductwise(startDateTime, applicationUser.OrgId.ToString());
            }

            if (loadOut.Any())
            {
                loadOutList = loadOut.ToList();
            }

            //Get default price Book List
            var defaultPriceBook = PriceBookLineItemService.GetDefaultPriceBook(applicationUser.Id, applicationUser.OrgId.ToString());

            foreach (var ins in loadIn)
            {
                //Initialze and set rcpts qty to zero if no rcpts found
                var outs = loadOutList.Find(r => r.ProductId == ins.ProductId) ??
                           new StockProductQuantityTotals()
                           {
                               ProductId = ins.ProductId,
                               ProductName = ins.ProductName,
                               TotalQuantity = 0,
                               Value = 0
                           };

                var viewModel = new StockProductQuantityTotals();
                viewModel.ProductName = ins.ProductName;
                viewModel.ProductId = ins.ProductId;
                viewModel.TotalQuantity = ins.TotalQuantity - outs.TotalQuantity;
                decimal price = 0;
                var priceBookLineItem = defaultPriceBook.Find(p => p.ProductId == viewModel.ProductId);
                if(priceBookLineItem != null)
                {
                    price = priceBookLineItem.ListPrice;
                }
                viewModel.Value = price * viewModel.TotalQuantity;
                viewModelList.Add(viewModel);
            }

            return viewModelList;
        }

        #endregion Current Stock
    }
}