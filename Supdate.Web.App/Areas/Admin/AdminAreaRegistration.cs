using System.Web.Mvc;

namespace Supdate.Web.App.Areas.Admin
{
  public class AdminAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Admin";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.Routes.LowercaseUrls = true;
      context.Routes.MapMvcAttributeRoutes();
      context.MapRoute(
          "Admin_default",
          "Admin/{controller}/{action}/{id}",
          new { contoller = "Admin", action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
