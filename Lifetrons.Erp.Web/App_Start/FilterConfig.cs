using System;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Extensions;
using Lifetrons.Erp.Helpers;

namespace Lifetrons.Erp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {

            filters.Add(new ElmahHandledErrorLoggerFilter());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
            //filters.Add(new AuditAttribute());
            
            //Enable this after obtaining SSL certificate
            //filters.Add(new RequireHttpsAttribute()); 
            
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new NullableDateTimeBinder());
            
        }
    }
}
