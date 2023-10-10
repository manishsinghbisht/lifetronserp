using System.Web.Mvc;
using System.Web.Routing;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.qr");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{culture}/{controller}/{action}/{id}",
                defaults: new
                {
                    //culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional 
                    culture = CultureHelper.GetDefaultCulture(),
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                },
                namespaces: new[] { "Lifetrons.Erp.Controllers" });

            routes.MapRoute(
                "Default Catch all 404",
                "{controller}/{action}/{*catchall}",
                new { controller = "Error", action = "NotFound", area="" },
                namespaces: new[] { "Lifetrons.Erp.Controllers" });

            routes.MapRoute(
                "Default Catch all 404 with culture",
                "{culture}/{controller}/{action}/{*catchall}",
                new { controller = "Error", action = "NotFound", area = "" },
                namespaces: new[] { "Lifetrons.Erp.Controllers" });

            routes.MapRoute(
            "Error - 404",
            "NotFound",
            new { controller = "Error", action = "NotFound", area = "" }
            );

            routes.MapRoute(
                "Error - 500",
                "InternalServer",
                new { controller = "Error", action = "InternalServer", area = "" },
                namespaces: new[] { "Lifetrons.Erp.Controllers" }
                );
        }
    }
}
