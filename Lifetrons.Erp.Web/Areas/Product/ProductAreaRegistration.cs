using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Web.Areas.Product
{
    public class ProductAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Product";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Product_default",
            //    "Product/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
             name: "Product_default",
             url: "{culture}/Product/{controller}/{action}/{id}",
             defaults: new
             {
                 //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
                 culture = CultureHelper.GetDefaultCulture(),
                 controller = "Home",
                 action = "Index",
                 id = UrlParameter.Optional
             },
             namespaces: new[] { "Lifetrons.Erp.Product.Controllers" });

        }
    }
}