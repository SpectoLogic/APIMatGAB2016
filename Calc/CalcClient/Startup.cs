using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CalcClient.Startup))]
namespace CalcClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
