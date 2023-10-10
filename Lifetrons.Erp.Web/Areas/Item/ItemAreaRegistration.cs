using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Web.Areas.Item
{
    public class ItemAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Item";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Item_default",
            //    "Item/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
              name: "Item_default",
              url: "{culture}/Item/{controller}/{action}/{id}",
              defaults: new
              {
                  //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
                  culture = CultureHelper.GetDefaultCulture(),
                  controller = "Home",
                  action = "Index",
                  id = UrlParameter.Optional
              },
              namespaces: new[] { "Lifetrons.Erp.Controllers" });
        }
    }
}