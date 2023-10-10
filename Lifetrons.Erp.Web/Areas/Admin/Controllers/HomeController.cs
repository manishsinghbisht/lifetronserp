using System.Threading.Tasks;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Web.Controllers;
using Microsoft.Practices.Unity;

namespace Lifetrons.Erp.Admin.Controllers
{
    using Lifetrons.Erp.Helpers;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Web;
    using System.Web.Mvc;

    [EsAuthorize(Roles = "HRAdminAuthorize, HRAdminEdit, HRAdminView")]
    public class HomeController : BaseController
    {
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
            ViewBag.Message = "Lifetrons.Erp.in";

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

    }
}