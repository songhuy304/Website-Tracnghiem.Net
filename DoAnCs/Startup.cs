using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DoAnCs.Startup))]
namespace DoAnCs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
