using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ConciliacionesPip.Startup))]
namespace ConciliacionesPip
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
