using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Supdate.Business;
using Supdate.Business.DataSources;
using Supdate.Model;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("datasources")]
  public class DataSourcesController : AuthenticatedControllerBase
  {
    private readonly IExternalApiAuthManager _externalApiAuthManager;
    private readonly ICompanyManager _companyManager;
    private readonly IGoogleAuthorizer _googleAuthorizer;

    public DataSourcesController(IExternalApiAuthManager externalApiAuthManager, ICompanyManager companyManager, IGoogleAuthorizer googleAuthorizer, ListHelper listHelper)
      : base(ownerAccessOnly: true)
    {
      _externalApiAuthManager = externalApiAuthManager;
      _companyManager = companyManager;
      _googleAuthorizer = googleAuthorizer;
      ListHelper = listHelper;
    }

    [HttpGet]
    public ActionResult Index()
    {
      ListHelper.InitializeMetrics(CurrentUser);
      ViewBag.MappedMetricsCount = ListHelper.GetMetrics().Count(m => m.DataSourceId.HasValue == true);
      var model = _externalApiAuthManager.GetExternalApiAuths(CompanyId);
      return View(model);
    }

    [HttpPost]
    [Route("credentials/{uniqueId?}")]
    public ActionResult Credentials(ExternalApiAuth externalApiAuth, Guid uniqueId = default(Guid))
    {
      var x = ExternalApi.ChartMogul;

      if (ModelState.IsValid)
      {
        externalApiAuth.CompanyId = CompanyId;
        _externalApiAuthManager.SaveExternalApiAuth(externalApiAuth);
        this.SetNotificationMessage(NotificationType.Success, "Configuration successfully saved.");
        return Json(new { success = true });
      }
      return Json(new { success = false });
    }

    [HttpGet]
    [Route("connect/{uniqueId}")]
    public async Task<ActionResult> Connect(Guid uniqueId, int externalApiId)
    {
      var externalApi = ExternalApi.ChartMogul.GetById(externalApiId);
      var externalApiAuth = new ExternalApiAuth { ExternalApiId = externalApiId };

      // Replace with requested object
      if (uniqueId != Guid.Empty)
      {
        externalApiAuth = _externalApiAuthManager.GetExternalApiAuth(CompanyId, uniqueId);
      }

      switch (externalApi.ApiAuthorizationType)
      {
        case ExternalApiAuthorizationType.OAuth20:
          // Hard coded to Google for now
          var company = _companyManager.Get(CompanyId);
          Session["CompanyGuid"] = company.UniqueId.ToString();
          var authResult = await _googleAuthorizer.Authorize(company.UniqueId, externalApiAuth, Url.Action("SetGoogleAnalyticsSiteId"), CancellationToken.None);
          if (authResult.Credential == null)
          {
            return View("_startOAuth", new OAuthStartAuthorisation { ExternalApi = externalApi, StartUrl = authResult.RedirectUri });
          }

          try
          {
            var accountSummaries = await _googleAuthorizer.GetAccountSummaries(authResult);
            if (string.IsNullOrWhiteSpace(externalApiAuth.ConfigData))
            {
              // Default to first profile
              externalApiAuth.ConfigData = accountSummaries.items[0].webProperties[0].profiles[0].id;
              _externalApiAuthManager.Update(externalApiAuth);
            }
            return View("_manageOAuth", new GoogleOAuthConfigView { ExternalApi = externalApi, ExternalApiAuth = externalApiAuth, Accounts = accountSummaries });
          }
          catch
          {
            _externalApiAuthManager.Delete(externalApiAuth.Id);
            return Content("We couldn't retrieve your Google Analytics site list.\nTry refreshing the page and re-connecting to Google Analytics");
          }

        default:
          return View("_connect", externalApiAuth);
      }
    }

    [HttpGet]
    [Route("set-google-analytics-siteid")]
    public ActionResult SetGoogleAnalyticsSiteId()
    {
      return RedirectToAction("Index", new { donext = "set-google-analytics-siteid" });
    }

    [HttpPost]
    [Route("set-google-analytics-siteid")]
    public JsonResult SetGoogleAnalyticsSiteId(string siteId)
    {
      var externalApiAuth = _externalApiAuthManager.GetExternalApiAuth(CompanyId, ExternalApi.GoogleAnalytics.Id);
      externalApiAuth.ConfigData = siteId;
      _externalApiAuthManager.Update(externalApiAuth);
      return Json(new { success = true });
    }

    [HttpPost]
    [Route("test-credentials")]
    public async Task<JsonResult> TestCredentials(string token, string key, int externalApiId)
    {
      var res = await _externalApiAuthManager.GetApiManager(externalApiId).TestCredentials(token, key, externalApiId);
      return Json(new { success = true, result = res });
    }

    [HttpPost]
    public ActionResult DeleteCredentials(Guid externalapiauthId)
    {
      _externalApiAuthManager.DeleteExternalApiAuth(CompanyId, externalapiauthId);
      return Json(new { success = true });
    }

    public ActionResult Intro()
    {
      return View();
    }
  }
}
