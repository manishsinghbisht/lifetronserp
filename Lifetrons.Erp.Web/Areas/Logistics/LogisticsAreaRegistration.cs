using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Web.Areas.Logistics
{
    public class LogisticsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Logistics";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Logistics_default",
            //    "Logistics/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
              name: "Logistics_default",
              url: "{culture}/Logistics/{controller}/{action}/{id}",
              defaults: new
              {
                  //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
                  culture = CultureHelper.GetDefaultCulture(),
                  controller = "Home",
                  action = "Index",
                  id = UrlParameter.Optional
              },
              namespaces: new[] { "Lifetrons.Erp.Logistics.Controllers" });
        }
    }
}