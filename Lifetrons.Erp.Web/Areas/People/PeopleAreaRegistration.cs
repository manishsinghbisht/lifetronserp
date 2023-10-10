using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Web.Areas.People
{
    public class PeopleAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "People";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "People_default",
            //    "People/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
              name: "People_default",
              url: "{culture}/People/{controller}/{action}/{id}",
              defaults: new
              {
                  //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
                  culture = CultureHelper.GetDefaultCulture(),
                  controller = "Home",
                  action = "Index",
                  id = UrlParameter.Optional
              },
              namespaces: new[] { "Lifetrons.Erp.People.Controllers" });
        }
    }
}