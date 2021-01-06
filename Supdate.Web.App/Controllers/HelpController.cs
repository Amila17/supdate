using System.Web.Mvc;
using Supdate.Web.App.Controllers.Base;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("help")]
  public class HelpController : AuthenticatedControllerBase
  {
    // GET: Help
    public ActionResult Index()
    {
      return View();
    }
  }
}
