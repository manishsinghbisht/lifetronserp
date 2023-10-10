using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Web.Areas.Activity
{
    public class ActivityAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Activity";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Activities_default",
            //    "Activities/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
              name: "Activity_default",
              url: "{culture}/Activity/{controller}/{action}/{id}",
              defaults: new
              {
                  //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
                  culture = CultureHelper.GetDefaultCulture(),
                  controller = "Home",
                  action = "Index",
                  id = UrlParameter.Optional
              },
              namespaces: new[] { "Lifetrons.Erp.Activity.Controllers" });
        }
    }
}