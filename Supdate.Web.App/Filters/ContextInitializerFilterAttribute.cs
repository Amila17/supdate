using System.Web.Mvc;
using Supdate.Web.App.Controllers.Base;

namespace Supdate.Web.App.Filters
{
  public class ContextInitializerFilterAttribute : FilterAttribute, IActionFilter
  {
    public void OnActionExecuting(ActionExecutingContext filterContext)
    {
      if (filterContext.ActionDescriptor.GetCustomAttributes(typeof (AllowAnonymousAttribute), false).Length > 0)
      {
        return;
      }

      var authenticatedController = filterContext.Controller as AuthenticatedControllerBase;
      if (authenticatedController != null)
      {
        var initialised =  authenticatedController.InitializeContext();

        if (!initialised)
        {
          var url = new UrlHelper(filterContext.RequestContext);
          filterContext.Result = new RedirectResult(url.Action("NoCompanyToAccess", "Account"));
        }
      }
    }

    public void OnActionExecuted(ActionExecutedContext filterContext)
    {
      // No action intended here.
    }
  }
}
