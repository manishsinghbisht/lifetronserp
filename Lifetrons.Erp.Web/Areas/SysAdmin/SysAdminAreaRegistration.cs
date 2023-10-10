using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Web.Areas.SysAdmin
{
    public class SystemAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SysAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "System_default",
            //    "System/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
       name: "SysAdmin_default",
       url: "{culture}/SysAdmin/{controller}/{action}/{id}",
       defaults: new
       {
           //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
           culture = CultureHelper.GetDefaultCulture(),
           controller = "Home",
           action = "Index",
           id = UrlParameter.Optional
       },
       namespaces: new[] { "Lifetrons.Erp.SysAdmin.Controllers" });
        }
    }
}