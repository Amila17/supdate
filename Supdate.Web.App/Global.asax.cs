using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using StackExchange.Profiling;
using Supdate.Web.App.Models;

namespace Supdate.Web.App
{
  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);

      // Disable excessive header.
      MvcHandler.DisableMvcResponseHeader = true;

      // Register event handler.
      PreSendRequestHeaders += Application_PreSendRequestHeaders;
    }

    protected void Application_BeginRequest()
    {
      InitializeProfiler();
    }

    private void InitializeProfiler()
    {
      var doProfile = false;

      // Check if cookie already exists or if startProfile is found in the query string.
      if (CookieUtil.AppProfiler == "do")
      {
        doProfile = true;
      }
      else if (Request.QueryString["startProfiler"] != null)
      {
        CookieUtil.AppProfiler = "do";
        doProfile = true;
      }

      // Check if stopPrilie is found in the query string.
      if (Request.QueryString["stopProfiler"] != null)
      {
        CookieUtil.RemoveAppProfiler();
        doProfile = false;
      }

      if (doProfile)
      {
        MiniProfiler.Start();
      }
    }

    protected void Application_EndRequest()
    {
      MiniProfiler.Stop();
    }

    protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
    {
      var app = sender as HttpApplication;

      if (app != null && !app.Request.IsLocal && app.Context != null)
      {
        var headers = app.Context.Response.Headers;
        headers.Remove("Server");
        headers.Remove("X-AspNet-Version");
        headers.Remove("X-AspNetMvc-Version");
      }
    }
  }
}
