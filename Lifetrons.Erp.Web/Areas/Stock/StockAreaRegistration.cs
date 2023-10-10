using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Web.Areas.Stock
{
    public class StockAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Stock";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Stock_default",
            //    "Stock/{culture}/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "Lifetrons.Erp.Controllers" }
            //);

            context.MapRoute(
               name: "Stock_default",
               url: "{culture}/Stock/{controller}/{action}/{id}",
               defaults: new
               {
                   //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
                   culture = CultureHelper.GetDefaultCulture(),
                   controller = "Home",
                   action = "Index",
                   id = UrlParameter.Optional
               },
               namespaces: new[] { "Lifetrons.Erp.Stock.Controllers" });
        }
    }
}