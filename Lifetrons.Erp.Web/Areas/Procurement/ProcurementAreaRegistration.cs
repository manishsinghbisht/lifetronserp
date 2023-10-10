using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Web.Areas.Procurement
{
    public class ProcurementAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Procurement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Procurement_default",
            //    "Procurement/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
              name: "Procurement_default",
              url: "{culture}/Procurement/{controller}/{action}/{id}",
              defaults: new
              {
                  //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
                  culture = CultureHelper.GetDefaultCulture(),
                  controller = "Home",
                  action = "Index",
                  id = UrlParameter.Optional
              },
              namespaces: new[] { "Lifetrons.Erp.Procurement.Controllers" });
        }
    }
}