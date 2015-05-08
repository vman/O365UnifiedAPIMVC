using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(O365.Unified.API.Startup))]
namespace O365.Unified.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
