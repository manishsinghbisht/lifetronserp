using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp.Controllers
{
    public class BaseController : Controller
    {
        [ValidateAntiForgeryToken]
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {

            // Attempt to read the culture cookie from Request
            string cultureName = RouteData.Values["culture"] as string ??
                                 (Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null);

            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            if (RouteData.Values["culture"] as string != cultureName)
            {

                // Force a valid culture in the URL
                RouteData.Values["culture"] = cultureName.ToLowerInvariant(); // lower case too

                // Redirect user
                Response.RedirectToRoute(RouteData.Values);
            }

            // Modify current thread's cultures            
            if (cultureName != null)
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);

            //var currentCulture = Thread.CurrentThread.CurrentCulture;
            //var culture = (CultureInfo)currentCulture.Clone();
            //culture.DateTimeFormat.ShortDatePattern = "yy/MM/dd";
            //culture.DateTimeFormat.ShortTimePattern = "hh:mm A";
            //Thread.CurrentThread.CurrentCulture = culture;
            
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern = "hh:mm A";
            if (Thread.CurrentThread.CurrentUICulture.Name.ToLower() == "hi-in" ||
                Thread.CurrentThread.CurrentUICulture.Name.ToLower() == "cs-cz" ||
                Thread.CurrentThread.CurrentUICulture.Name.ToLower() == "de-de" 
                )
            {
                Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            }
            if (Thread.CurrentThread.CurrentUICulture.Name.ToLower() == "en-us")
            {
                Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";
            }


            return base.BeginExecuteCore(callback, state);
        }

        #region Handle Exception

        protected string HandleDbEntityValidationException(DbEntityValidationException dbEntityValidationException)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(dbEntityValidationException));
            // Retrieve the error messages as a list of strings.
            var errorMessages = dbEntityValidationException.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

            // Join the list to a single string.
            var fullErrorMessage = string.Join("; ", errorMessages);

            // Combine the original exception message with the new one.
            var exceptionMessage = string.Concat(dbEntityValidationException.Message, " The validation errors are: ", fullErrorMessage);

            // Throw a new DbEntityValidationException with the improved exception message.
            TempData["CustomErrorMessage"] = exceptionMessage;
            TempData["CustomErrorDetail"] = dbEntityValidationException.InnerException.Message;
            AddErrors(exceptionMessage);

            return exceptionMessage;
        }

        protected void HandleException(Exception exception)
        {
            //Log the error
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(exception));
            TempData["CustomErrorMessage"] = exception.Message;
            TempData["CustomErrorDetail"] = "";

            AddErrors(exception.Message);
        }

        protected void AddErrors(string errorMessage)
        {
            ModelState.AddModelError("", errorMessage + Lifetrons.Erp.Data.Helper.SysSeparator + DateTime.UtcNow);
        }

        #endregion Handle Exception
    }
}