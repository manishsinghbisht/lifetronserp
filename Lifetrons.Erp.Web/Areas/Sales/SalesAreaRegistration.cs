using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Web.Areas.Sales
{
    public class SalesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Sales";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //    "Sales_default",
            //    "Sales/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
        name: "Sales_default",
        url: "{culture}/Sales/{controller}/{action}/{id}",
        defaults: new
        {
            //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
            culture = CultureHelper.GetDefaultCulture(),
            controller = "Home",
            action = "Index",
            id = UrlParameter.Optional
        },
        namespaces: new[] { "Lifetrons.Erp.Sales.Controllers" });
        }
    }
}