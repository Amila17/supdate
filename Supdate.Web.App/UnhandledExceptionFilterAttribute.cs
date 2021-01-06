using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using StackExchange.Exceptional;

namespace Supdate.Web.App
{
  public class UnhandledExceptionFilterAttribute : HandleErrorAttribute
  {
    public override void OnException(ExceptionContext filterContext)
    {
      // This error occurs when you login as user A, press the back button, land on the login page and try to login as user B.
      if (filterContext.Exception is HttpAntiForgeryException)
      {
        filterContext.ExceptionHandled = true;
        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Account", area = string.Empty }));
      }
      else
      {
        ErrorStore.LogException(filterContext.Exception, HttpContext.Current);
      }

      base.OnException(filterContext);
    }
  }
}
