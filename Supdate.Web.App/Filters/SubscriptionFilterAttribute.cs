using System.Web.Mvc;
using Supdate.Web.App.Controllers.Base;

namespace Supdate.Web.App.Filters
{
  public class SubscriptionFilter : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);

      // Confirm that this is an authenticated controller.
      var authenticatedController = filterContext.Controller as AuthenticatedControllerBase;

      if (authenticatedController != null)
      {
        var initialised = authenticatedController.InitializeContext();

        if (initialised && !authenticatedController.IsSubscriptionActive)
        {
          var url = new UrlHelper(filterContext.RequestContext);
          filterContext.Result = new RedirectResult(url.Action("PremiumFeature", "Home"));
        }
      }
    }
  }
}
