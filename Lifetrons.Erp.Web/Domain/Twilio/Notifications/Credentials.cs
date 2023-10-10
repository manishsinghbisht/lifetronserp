using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Lifetrons.Erp.Web.Domain.Twilio.Notifications
{
    public class Credentials
    {
        public static string TwilioAccountSid = WebConfigurationManager.AppSettings["TwilioAccountSid"];
        public static string TwilioAuthToken = WebConfigurationManager.AppSettings["TwilioAuthToken"];
        public static string TwilioPhoneNumber = WebConfigurationManager.AppSettings["TwilioPhoneNumber"];
    }
}