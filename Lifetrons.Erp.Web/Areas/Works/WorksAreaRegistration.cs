using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Web.Areas.Production
{
    public class WorksAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Works";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Production_default",
            //    "Production/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
           name: "Works_default",
           url: "{culture}/Works/{controller}/{action}/{id}",
           defaults: new
           {
               //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
               culture = CultureHelper.GetDefaultCulture(),
               controller = "Home",
               action = "Index",
               id = UrlParameter.Optional
           },
           namespaces: new[] { "Lifetrons.Erp.Works.Controllers" });
        }
    }
}