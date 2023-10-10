using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;
using Lifetrons.Erp.Service;
using Elmah;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Provider;
using Microsoft.Practices.Unity;
using Repository.Pattern.Infrastructure;

namespace Lifetrons.Erp.Helpers
{
    public static class ExtensionMethods
    {
        public static string TimeZone(this IPrincipal principal)
        {
            // do some database access 
            var applicationUser = new AccountController().GetUserById(principal.Identity.GetUserId());
            return applicationUser.TimeZone;
        }
    }


    public class ControllerHelper
    {
        public const string AuditData = "Audit_Data";

       public static AspNetUser GetAspNetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            var user = new AccountController().GetUserById(userId);

            var aspNetUser = new AspNetUser
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AuthenticatedEmail = user.AuthenticatedEmail,
                Mobile = user.Mobile,
                OrgId = user.OrgId,
                Culture = user.Culture,
                TimeZone = user.TimeZone,
            };

            user = null;

            return aspNetUser;
        }

        public static DataTable ChangeDateTimeInDataTableToUserTimeZone(DataTable dataTable, string timeZone)
        {
            foreach (DataColumn column in dataTable.Columns)
            {
                if (column.GetType() == System.Type.GetType("System.DateTime"))
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        row[column] = ControllerHelper.ConvertDateTimeToUtc(Convert.ToDateTime(row[column]), timeZone);
                    }
                }
            }

            return dataTable;
        }

        public static DateTime? ConvertDateTimeFromUtc(DateTime? utcDateTime, string timeZone)
        {
            if (utcDateTime == null) return null;
            if (utcDateTime.Value.Year == 0001) return null;

            return ConvertDateTimeFromUtc(Convert.ToDateTime(utcDateTime), timeZone);
        }

        public static DateTime? ConvertDateTimeToUtc(DateTime? localDateTime, string timeZone)
        {
            if (localDateTime == null) return null;
            if (localDateTime.Value.Year == 0001) return null;

            return ConvertDateTimeToUtc(Convert.ToDateTime(localDateTime), timeZone);
        }

        public static DateTime ConvertDateTimeFromUtc(DateTime utcDateTime, string timeZone)
        {
            try
            {
                if (utcDateTime.Year == 0001) utcDateTime = DateTime.UtcNow;
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                var cultureSpecificDate = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);
                return cultureSpecificDate;
            }
            catch (TimeZoneNotFoundException exception)
            {
                Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception));
                return utcDateTime;
            }
        }

        public static DateTime ConvertDateTimeToUtc(DateTime localDateTime, string timeZone)
        {
            try
            {
                if (localDateTime.Year == 0001) return DateTime.UtcNow;
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                var cultureSpecificDate = TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZoneInfo);
                return cultureSpecificDate;
            }
            catch (TimeZoneNotFoundException exception)
            {
                Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exception));
                return localDateTime;
            }
        }

        public ComparisonResult Compare(Object obj1, Object obj2, List<string> membersToCompare)
        {
            //By default for performance reasons, Compare .NET Objects only detects the first difference.  To capture all differences set Config.MaxDifferences to the maximum differences desired. 
            //After the comparison, the differences are in the Differences list or in the DifferencesString properties. 
            //By default, a deep comparison is performed.  To perform a shallow comparison, set Config.CompareChildren = false 
            //By default, private properties and fields are not compared.  Set ComparePrivateProperties and ComparePrivateFields to true to override this behavior. 
            //By default an exception is thrown when class types are different.  To ignore object types, set Config.IgnoreObjectTypes to true
            
            var compareLogic = new CompareLogic {Config = {ComparePrivateFields = true, MaxDifferences = 50}};
            compareLogic.Config.MembersToIgnore = new List<string>()
            {
                "CreatedDate",
                "CreatedBy",
                "ModifiedDate",
                "ModifiedBy",
                "OrgId",
                "ObjectState",
                "TimeStamp"
            };
            compareLogic.Config.MembersToInclude = membersToCompare;
            compareLogic.Config.AttributesToIgnore.Add(typeof(ExcludeFromEqualityComparison));
            ComparisonResult result = compareLogic.Compare(obj1, obj2);

            return result;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Use [SessionExpireFilterAttribute] atribute on Action or Classes to use, if global filter registration does not works.
            // Update Global filter puhing site into infinite loop. Do not register filter globally without necessary cod changes in fliter class

            HttpContext ctx = HttpContext.Current;

            // check if session is supported
            if (filterContext.HttpContext.Session != null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    // For AJAX requests, we're overriding the returned JSON result with a simple string,
                    // indicating to the calling JavaScript code that a redirect should be performed.
                    filterContext.Result = new JsonResult { Data = "_Logon_" };
                }
                // check if a new session id was generated. Or If the browser session or authentication session has expired...
                if (filterContext.HttpContext.Session.IsNewSession || !filterContext.HttpContext.Request.IsAuthenticated)
                //if (!filterContext.HttpContext.Request.IsAuthenticated)
                //if (filterContext.HttpContext.Session.IsNewSession)
                {
                    // If it says it is a new session, but an existing cookie exists, then it must have timed out
                    string sessionCookie = filterContext.HttpContext.Request.Headers["Cookie"];
                    //if ((null != sessionCookie) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                    //{
                    //    filterContext.Result = OnSessionExpiryRedirectResult(filterContext);
                    //}
                    filterContext.Result = OnSessionExpiryRedirectResult(filterContext);
                }
            }

            base.OnActionExecuting(filterContext);
        }

        private ActionResult OnSessionExpiryRedirectResult(ActionExecutingContext filterContext)
        {
            return new RedirectToRouteResult(new RouteValueDictionary {
                                                                        { "Controller", "Error" },
                                                                        { "Action", "SessionExpired" }}
                                                                        );
        }
    }

    
}