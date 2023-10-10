using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Lifetrons.Erp.Web.Domain.Twilio.MarketingNotifications
{
    public class SubscribersNotificationConfiguration
    {
        public class Credentials
        {
            public static string AccountSID
            {
                get { return WebConfigurationManager.AppSettings["TwilioAccountSid"]; }
            }

            public static string AuthToken
            {
                get { return WebConfigurationManager.AppSettings["TwilioAuthToken"]; }
            }
        }


        public class PhoneNumbers
        {
            public static string Twilio
            {
                get { return WebConfigurationManager.AppSettings["TwilioPhoneNumber"]; }
            }
        }
    }
}