using System.Configuration;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using Supdate.Util;

[assembly: OwinStartupAttribute(typeof(Supdate.Web.App.Startup))]
namespace Supdate.Web.App
{
  public partial class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      ConfigureAuth(app);

      var connectionStringName = ConfigUtil.CacheConnectionStringName;
      var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
      GlobalHost.DependencyResolver.UseRedis(new RedisScaleoutConfiguration(connectionString, connectionStringName));

      app.MapSignalR();
    }
  }
}
