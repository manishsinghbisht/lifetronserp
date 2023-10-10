using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Web.Areas.File
{
    public class FileAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "File";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "File_default",
            //    "File/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
             name: "File_default",
             url: "{culture}/File/{controller}/{action}/{id}",
             defaults: new
             {
                  //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
                  culture = CultureHelper.GetDefaultCulture(),
                 controller = "Home",
                 action = "Index",
                 id = UrlParameter.Optional
             },
             namespaces: new[] { "Lifetrons.Erp.File.Controllers" });
        }
    }
}