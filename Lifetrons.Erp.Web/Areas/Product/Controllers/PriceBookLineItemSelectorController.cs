using System;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Service;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace Lifetrons.Erp.Product.Controllers
{
    [EsAuthorize]
    public class PriceBookLineItemSelectorController : BaseController
    {
        private readonly IPriceBookLineItemService _service;
        private readonly IPriceBookService _priceBookService;

        public PriceBookLineItemSelectorController(IPriceBookLineItemService service, IPriceBookService priceBookService)
        {
            _service = service;
            _priceBookService = priceBookService;
        }

        public ActionResult PriceBookLineItemDisplay(string priceBookId, string selectedProductId, bool showSelectLink)
        {
            //How to get current URL:
            //- Without querystring: Request.Url.GetLeftPart(UriPartial.Path)
            //- With querystring: Request.Url.PathAndQuery

            //TempData["returnAction"] = returnAction;
            //TempData["returnController"] = returnController;
            TempData["ShowSelectLink"] = showSelectLink;

            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            
            //If PriceBookId is NOT available, load empty product dropdownlist
            ViewBag.ProductId = new SelectList("");

            //If PriceBook is available
            if (!string.IsNullOrEmpty(priceBookId))
            {
                ViewBag.PriceBookId = priceBookId;
                ViewBag.PriceBookName = (_priceBookService.Find(priceBookId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
              
                if (showSelectLink)
                {
                    ViewBag.ProductId =
                        new SelectList(_service.SelectLineItems(priceBookId, applicationUser.Id, applicationUser.OrgId.ToString()),
                            "ProductId", "Product.Name", selectedProductId);
                }
                else if (!string.IsNullOrEmpty(selectedProductId))
                {
                    ViewBag.ProductId = selectedProductId;
                    ViewBag.ProductName = (_service.Find(priceBookId, selectedProductId, applicationUser.Id, applicationUser.OrgId.ToString())).Product.Name;
                }
            }

            var m = new PriceBookLineItemSelectorModel();
            return PartialView("PriceBookLineItemDisplay", m);
        }

        [HttpGet]
        public async Task<ActionResult> PriceBookLineItemSelector(string priceBookId, string returnAction, string returnController, string returnUrl)
        {
            //How to get current URL:
            //- Without querystring: Request.Url.GetLeftPart(UriPartial.Path)
            //- With querystring: Request.Url.PathAndQuery

            var model = new List<PriceBookLineItemSelectorModel>();
            TempData["returnAction"] = returnAction;
            TempData["returnController"] = returnController;
            TempData["returnUrl"] = returnUrl;
            
            var applicationUser = new AccountController().GetUserById(User.Identity.GetUserId());
            if (string.IsNullOrEmpty(priceBookId))
            {
                ViewBag.PriceBookId = new SelectList(await _priceBookService.SelectAsync(applicationUser.Id, applicationUser.OrgId.ToString()), "Id", "Name");
                TempData["EnablePriceBookSelection"] = "true";
                TempData["PriceBookName"] = string.Empty;
            }
            else
            {
                ViewBag.PriceBookId = priceBookId;
                TempData["EnablePriceBookSelection"] = "false";
                TempData["PriceBookName"] = (await _priceBookService.FindAsync(priceBookId, applicationUser.Id, applicationUser.OrgId.ToString())).Name;
                var lineItems = await _service.SelectAsyncLineItems(priceBookId, applicationUser.Id, applicationUser.OrgId.ToString());
                lineItems.ForEach(li =>
                {
                    var m = new PriceBookLineItemSelectorModel
                    {
                        ProductId = li.ProductId,
                        ProductName = li.Product.Name,
                        PriceBookId = li.PriceBookId,
                        PriceBookName = li.PriceBook.Name,
                        ProductCode = li.Product.Code,
                        ListPrice = li.ListPrice,
                        ShrtDesc = li.ShrtDesc,
                        ImageAddr = li.Organization.ImagePath + li.Product.Code + "_thumbnail.jpg",
                        IsSelected = false,
                        ReturnAction = returnAction,
                        ReturnControl = returnController,
                        ReturnUrl = returnUrl + "&ProductId=" + li.ProductId
                    };
                    model.Add(m);
                });
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult PriceBookLineItemSelectorPost(string priceBookId, string returnAction,
            string returnController, string returnUrl)
        {
            returnUrl = returnUrl + "&priceBookId=" + priceBookId;
            return RedirectToAction("PriceBookLineItemSelector", new {priceBookId = priceBookId, returnAction = returnAction, returnController = returnController, returnUrl = returnUrl});
        }
        public ActionResult PriceBookLineItemSelectorManageSelection(string productId, string returnUrl)
        {
            TempData["PriceBookLineItemSelectorSelectedId"] = productId;
            return Redirect(returnUrl);
        }
    }
}