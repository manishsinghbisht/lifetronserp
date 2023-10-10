using Lifetrons.Erp.Models;
using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Lifetrons.Erp.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuditAttribute : ActionFilterAttribute
    {
        //Our value to handle our AuditingLevel
        public int AuditingLevel { get; set; }

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    //Stores the Request in an Accessible object
        //    var request = filterContext.HttpContext.Request;

        //    //Generate the appropriate key based on the user's Authentication Cookie
        //    //This is overkill as you should be able to use the Authorization Key from
        //    //Forms Authentication to handle this. 
        //    var sessionIdentifier = (request.IsAuthenticated) ? filterContext.HttpContext.Session.SessionID : "Anonymous";

        //    //Generate an audit
        //    Audit audit = new Audit()
        //    {
        //        SessionId = sessionIdentifier,
        //        AuditId = Guid.NewGuid(),
        //        IpAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress,
        //        UrlAccessed = request.RawUrl,
        //        TimeAccessed = DateTime.UtcNow,
        //        UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
        //        Data = SerializeRequest(request)
        //    };

        //    //Stores the Audit in the Database
        //    var context = new AuditingContext();
        //    context.AuditRecords.Add(audit);
        //    context.SaveChanges();

        //    base.OnActionExecuting(filterContext);
        //}

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //Stores the Request in an Accessible object
            var request = filterContext.HttpContext.Request;
            
            //Generate the appropriate key based on the user's Authentication Cookie
            //This is overkill as you should be able to use the Authorization Key from
            //Forms Authentication to handle this. 
            var sessionIdentifier = (request.IsAuthenticated) ? filterContext.HttpContext.Session.SessionID : "Anonymous";

            //Generate an audit
            Audit audit = new Audit()
            {
                SessionId = sessionIdentifier,
                AuditId = Guid.NewGuid(),
                IpAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress,
                UrlAccessed = Convert.ToString(request.Url.PathAndQuery),
                TimeAccessed = DateTime.UtcNow,
                UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
                Data = SerializeRequest(request, Convert.ToString(filterContext.HttpContext.Items[ControllerHelper.AuditData]))
            };

            //Stores the Audit in the Database
            var context = new AuditingContext();
            context.AuditRecords.Add(audit);
            context.SaveChanges();

            base.OnActionExecuted(filterContext);
        }

        //This will serialize the Request object based on the level that you determine
        private string SerializeRequest(HttpRequestBase request, string auditData = "")
        {
            switch (AuditingLevel)
            {
                //No Request Data will be serialized
                case 0:
                default:
                    return "";
                //Basic Request Serialization - just stores Data
                case 1:
                    return Json.Encode(new { customData = auditData });
                //Middle Level - Customize to your Preferences
                case 2:
                    return Json.Encode(new { request.Cookies, request.Headers, request.Files, request.Form, request.QueryString, request.Params, customData = auditData });
                //Highest Level - Serialize the entire Request object
                case 3:
                    //We can't simply just Encode the entire request string due to circular references as well
                    //as objects that cannot "simply" be serialized such as Streams, References etc.
                    //return Json.Encode(request);
                    return Json.Encode(new { request.Cookies, request.Headers, request.Files, request.Form, request.QueryString, request.Params, customData = auditData });
            }
        }
     }

}
