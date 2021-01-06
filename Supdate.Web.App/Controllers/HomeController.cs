using System.Linq;
using System.Web.Mvc;
using Supdate.Business;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("home")]
  [Authorize]
  public class HomeController : AuthenticatedControllerBase
  {
    private readonly IReportManager _reportManager;
    private readonly IMetricManager _metricManager;
    private readonly ICompanyManager _companyManager;

    public HomeController(IReportManager reportManager, IMetricManager metricManager, ICompanyManager companyManager, ListHelper listHelper)
    {
      _reportManager = reportManager;
      _metricManager = metricManager;
      _companyManager = companyManager;
      ListHelper = listHelper;
    }

    [HttpGet]
    public ActionResult Index()
    {
      // Initialize the master data for the company.
      InitializeMasterLists(CurrentUser);
      ListHelper.InitializeMetricDataSources(CompanyId);

      // Get report summaries.
      var reportSummaries = _reportManager.GetReportSummaryList(CompanyId, CurrentUser, ListHelper.GetMetrics().Count()).ToList();

      var metricViews = _metricManager.GetMetricViews(CompanyId, null, CurrentUser).ToList();
      // Remove metrics that have no current value
      metricViews.RemoveAll(m => m.LatestValue.HasValue == false);

      var showWizard = IsCompanyAdmin && !MasterAreas.Any();
      var promoteTeams = IsCompanyAdmin && IsSubscriptionActive && !_companyManager.GetCompanyTeamMembers(CompanyId).Any();
      var promoteDataSources = IsCompanyAdmin && IsSubscriptionActive && !ListHelper.MetricDataSourceList.Any();

      // Don't show all 3 promo panels at once. Disable promoteDataSources if both others are enabled
      if (showWizard && promoteTeams)
      {
        promoteDataSources = false;
      }

      return View(new Dashboard { ReportSummaries = reportSummaries.ToList(), MetricViews = metricViews, ListHelper = ListHelper, ShowWizard = showWizard, PromoteTeams = promoteTeams, PromoteDataSources = promoteDataSources });
    }

    [HttpGet]
    [AllowAnonymous]
    public ActionResult Error()
    {
      return View();
    }

    [HttpGet]
    public ActionResult PremiumFeature()
    {
      this.SetNotificationMessage(NotificationType.Error, "You need a subscription to access Premium Features.");
      return RedirectToAction("Index", "Billing");
    }

    [HttpGet]
    public ActionResult HeadsUp()
    {
      return View();
    }

  }
}
