using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lifetrons.Erp.Controllers
{
    [AllowAnonymous]
    [HandleError]
    public class ErrorController : BaseController
    {
        [AllowAnonymous]
        public ActionResult SessionExpired()
        {
            return View();
        }

        [AllowAnonymous]
        public string UserInactive()
        {
            return "Services to this username has been temporarily suspended. Please contact your administrator or connect with us. Our contact info is available at ContactUs page. ";
        }

        // GET: /Error/
        [AllowAnonymous]
        public ActionResult NotFound(string aspxerrorpath = "")
        {
            Response.TrySkipIisCustomErrors = true;
            Url.Action("Login", "Account");
            return View();
        }

        [AllowAnonymous]
        public ActionResult InternalServer(string aspxerrorpath = "")
        {
            Response.TrySkipIisCustomErrors = true;
            Url.Action("Login", "Account");
            return View();
        }

        [AllowAnonymous]
        public ActionResult Error(string message = "")
        {
            TempData["CustomErrorMessage"] = message;
            TempData["CustomErrorDetail"] = message;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Index(string message = "")
        {
            TempData["CustomErrorMessage"] = message;
            TempData["CustomErrorDetail"] = message;

            return View();
        }

        [AllowAnonymous]
        protected override void HandleUnknownAction(string actionName)
        {
            // If controller is ErrorController dont 'nest' exceptions
            if (this.GetType() != typeof (ErrorController))
            {
                Index();
            }
            else
            {
                this.NotFound();
            }
                
        }

	}
}