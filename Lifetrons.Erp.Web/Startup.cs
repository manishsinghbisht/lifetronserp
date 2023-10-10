using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Lifetrons.Erp.Web.Startup))]

namespace Lifetrons.Erp.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
