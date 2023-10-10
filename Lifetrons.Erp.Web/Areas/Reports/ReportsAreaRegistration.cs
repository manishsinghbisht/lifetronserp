using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Web.Areas.Reports
{
    public class ReportsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Reports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Reports_default",
            //    "Reports/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
          name: "Reports_default",
          url: "{culture}/Reports/{controller}/{action}/{id}",
          defaults: new
          {
              //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
              culture = CultureHelper.GetDefaultCulture(),
              controller = "Home",
              action = "Index",
              id = UrlParameter.Optional
          },
          namespaces: new[] { "Lifetrons.Erp.Reports.Controllers" });
        }
    }
}