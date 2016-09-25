using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebLayer.Startup))]
namespace WebLayer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
