using System.Threading.Tasks;
using Lifetrons.Erp.Web.Controllers;
using Microsoft.Practices.Unity;

namespace Lifetrons.Erp.Controllers
{
    using App_Start;
    using Lifetrons.Erp.Helpers;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Timers;
    using System.Web;
    using System.Web.Mvc;

    public class HomeController : BaseController
    {
        [Dependency]
        public Lifetrons.Erp.File.Controllers.FileController objFileController { get; set; }

        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated) return View();
            var userId = User.Identity.GetUserId();
            var acct = new AccountController();
            var user = acct.GetUserById(userId);
            if (user.OrgId == null)
            {
                return RedirectToAction("ManageOrganization", "Organization", new { area = "SysAdmin" });
            }
            //return RedirectToAction("Modules", "Home");

            return View();
        }

        [EsAuthorize]
        public ActionResult Modules()
        {
            ViewBag.Message = "Modules";

            return View();
        }

        [EsAuthorize]
        public ActionResult ModuleSelector()
        {
            ViewBag.Message = "Modules";

            return PartialView();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Lifetrons";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Lifetrons";

            return View();
        }

        [AllowAnonymous]
        public ActionResult ProcessSteps()
        {
            ViewBag.Message = "Lifetrons";

            return View();
        }

        [AllowAnonymous]
        public ActionResult VideoHelp()
        {
            ViewBag.Message = "Lifetrons";

            return View();
        }

        [AllowAnonymous]
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture;  // set culture

            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
            {
                cookie.Value = culture; // update cookie value
            }
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.UtcNow.AddYears(1);
            }

            Response.Cookies.Add(cookie);

            return RedirectToAction("Index");

        }


    }
}