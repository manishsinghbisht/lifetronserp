using System.Threading;
using System.Web;
using System.Web.Routing;
using Lifetrons.Erp.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.Owin.Security.Google;
using System.Timers;
using System;
using Microsoft.Practices.Unity;

namespace Lifetrons.Erp.Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            string culture = Thread.CurrentThread.CurrentCulture.Name;

            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/" + culture + "/Account/Login")
            });
            
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication();
            var googleAuthOptions = new GoogleAuthenticationOptions() { };


            if (HttpContext.Current.IsDebuggingEnabled)
            {
                //Local debug
                app.UseGoogleAuthentication(
                    clientId: "257286200176-92m5pifrg7jbbe5fsob19v44a3jou3id.apps.googleusercontent.com",
                    clientSecret: "TuPkSQcmsyNq-MfnxjjiD4e8");
            }
            else
            {
                //Production
                app.UseGoogleAuthentication(
                    clientId: "474934194119-uee6inblje730plim38cen8gi0acvnm8.apps.googleusercontent.com",
                    clientSecret: "5yXzoiZyHB9-JKcwTtNMIvnw");
            }
        }
    }
}