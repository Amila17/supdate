using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Supdate.Business;
using Supdate.Model;
using Supdate.Web.App.Controllers.Base;
using Supdate.Web.App.Models;

namespace Supdate.Web.App.Controllers
{
  [RoutePrefix("metrics")]
  public class MetricsController : AuthenticatedControllerBase
  {
    private readonly ICompanyManager _companyManager;
    private readonly IMetricManager _metricManager;

    public MetricsController(ICompanyManager companyManager,
      IMetricManager metricManager, ListHelper listHelper)
    {
      _companyManager = companyManager;
      _metricManager = metricManager;
      ListHelper = listHelper;
    }

    [Route("{areaUniqueId?}")]
    [HttpGet]
    public ActionResult Index(Guid? areaUniqueId)
    {
      ListHelper.InitializeAreas(CurrentUser);
      ListHelper.InitializeGraphTypesList();
      ListHelper.InitializeMetrics(CurrentUser);

      var metricList = ListHelper.GetMetrics();

      //enable filter list if more than one reporting area
      ViewBag.FilterListShow = (CurrentUser.AccessibleAreaIds.Count() > 1);
      ViewBag.FilterListTitle = "All Metrics";
      ViewBag.FilterListIndex = areaUniqueId.HasValue ? areaUniqueId.Value.ToString() : string.Empty;

      // filter list of metrics if filter is active
      if (areaUniqueId.HasValue)
      {
        var selectedArea = ListHelper.GetAreas().SingleOrDefault(a => a.UniqueId == areaUniqueId);
        ViewBag.FilterListTitle = selectedArea.Name + " Metrics";
        ViewBag.SelectAreaName = selectedArea.Name;
        var areaId = selectedArea.Id;
        metricList = metricList.Where(a => a.AreaId == areaId);

      }
      return View(new MetricSettings() { Metric = new Metric(), Metrics = metricList.ToList(), ListHelper = ListHelper });
    }

    [HttpPost]
    [Route("details/{uniqueId?}")]
    public ActionResult Details(MetricSettings metricSettings, Guid uniqueId = default(Guid))
    {
      metricSettings.Metric.UniqueId = uniqueId;
      if (ModelState.IsValid)
      {
        metricSettings.Metric.CompanyId = CompanyId;

        if (metricSettings.Metric.AreaId == -2)
        {
          metricSettings.Metric.AreaId = -1;
          // created via Wizard, look for an area called "KPIs" and assign metric to that area if it exists
          ListHelper.InitializeAreas(CurrentUser);
          var area = ListHelper.GetAreas().FirstOrDefault(a => a.Name == "KPIs");
          if (area != null)
          {
            metricSettings.Metric.AreaId = area.Id;
          }
        }

        if (metricSettings.Metric.AreaId == -1)
        {
          metricSettings.Metric.AreaId = null;
        }
        else
        {
          if (!CurrentUser.AccessibleAreaIds.Contains(metricSettings.Metric.AreaId.Value))
          {
            throw new HttpException(400, "Bad Request");
          }
        }

        if (metricSettings.Metric.DataSourceId == -1)
        {
          metricSettings.Metric.DataSourceId = null;
        }

        this.SetNotificationMessage(NotificationType.Success, "Metric successfully saved.");

        return
          Json(metricSettings.Metric.UniqueId == Guid.Empty
            ? new { success = _metricManager.Create(metricSettings.Metric) != null }
            : new { success = _metricManager.Update(metricSettings.Metric) != null });
      }
      return Json(new { success = false });
    }

    [HttpGet]
    [Route("details/{uniqueId}")]
    public ActionResult Details(Guid uniqueId, Guid? areaUniqueId)
    {
      ListHelper.InitializeAreas(CurrentUser);
      ListHelper.InitializeMetrics(CurrentUser);
      ListHelper.InitializeGraphTypesList();
      ListHelper.InitializeMetricDataSources(CompanyId);

      // a new, empty metric
      int? areaId = null;
      if (areaUniqueId.HasValue)
      {
        areaId = ListHelper.GetAreas().SingleOrDefault(a => a.UniqueId == areaUniqueId).Id;
      }
      var metric = new Metric { AreaId = areaId };

      // replace with requested metric
      if (uniqueId != Guid.Empty)
      {
        metric = _metricManager.GetMetric(CompanyId, uniqueId);
      }

      return View("_details", new MetricViewModel { Metric = metric, ListHelper = ListHelper });
    }

    [Route("data")]
    [HttpGet]
    public ActionResult Data()
    {
      ListHelper.InitializeMetrics(CurrentUser);
      ListHelper.InitializeMetricDataSources(CompanyId);

      var metricList = ListHelper.GetMetrics();

      var data = _metricManager.GetDataPointsForPeriod(CompanyId, DateTime.UtcNow.Year, null, ListHelper.MetricIds());

      ViewBag.ShowImportButton = false;
      if (ListHelper.GetMetricDataSources().Any() && ListHelper.GetMetrics().Any(m => m.DataSourceId.HasValue))
      {
        ViewBag.ShowImportButton = true;
      }

      return View(new MetricDataViewModel() { Year = DateTime.UtcNow.Year, StartIndex = 0, Metrics = metricList.ToList(), Data = data, });

    }
    [Route("data/year/{year}")]
    [HttpGet]
    public ActionResult AddData(int year, int startIndex)
    {
      ListHelper.InitializeMetrics(CurrentUser);
      var metricList = ListHelper.GetMetrics();
      var data = _metricManager.GetDataPointsForPeriod(CompanyId, year, null, ListHelper.MetricIds());

      return View("_dataTable", new MetricDataViewModel() { Year = year, StartIndex = startIndex, Metrics = metricList.ToList(), Data = data, });

    }

    [Route("data")]
    [HttpPost]
    public ActionResult Data(IList<MetricDataPoint> data)
    {
      ListHelper.InitializeMetrics(CurrentUser);
      _metricManager.SaveMetricDataPoints(CompanyId, data.ToList(), ListHelper.MetricIds());
      return Json(new { success = true });
    }

    [Route("save-display-order")]
    [HttpPost]
    public JsonResult SaveMetricDisplayOrder(string displayOrder)
    {
      var displayOrders = JsonConvert.DeserializeObject<List<EntityDisplayOrder>>(displayOrder);
      var success = false;

      if (ModelState.IsValid)
      {
        _metricManager.SaveDisplayOrder(CompanyId, displayOrders);
        success = true;
      }

      return Json(new { success });
    }

    [Route("delete")]
    public ActionResult Delete(Guid metricId)
    {
      _metricManager.DeleteMetric(CompanyId, metricId);
      return Json(new { success = true });
    }
    private DateTime GetCompanyStartDate(int companyId)
    {
      var company = _companyManager.Get(companyId);

      return company.StartMonth ?? DateTime.Now;
    }
    private string ViewToString(string viewName, dynamic model)
    {
      ViewData.Model = model;
      using (var sw = new StringWriter())
      {
        var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
        var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
        viewResult.View.Render(viewContext, sw);
        return sw.GetStringBuilder().ToString();
      }
    }

    [Route("graphs/all")]
    public JsonResult GraphData()
    {
      var graphData = _metricManager.GraphData(CompanyId);
      if (!CurrentUser.IsCompanyAdmin)
      {
        // remove metrics team members shouldn't see
        ListHelper.InitializeMetrics(CurrentUser);
        var metricGuids = ListHelper.GetMetrics().Select(m => m.UniqueId).ToArray();
        graphData = graphData.Where(g => metricGuids.Contains(g.UniqueId));
      }
      JsonResult result = Json(graphData);
      result.MaxJsonLength = int.MaxValue;
      result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return result;
    }

    [Route("data/import")]
    [HttpPost]
    public async Task<ActionResult> ImportMetricData(int year, int? month)
    {
      if (!IsSubscriptionActive)
      {
        // Data Imports require a valdi subscription
        var errResult = new MetricDataImport
        {
          success = true,
          errors = new List<JsonError>
          {
            new JsonError{ErrorMessage = string.Format("You need a valid subscription to import data. <a href='{0}' class='link'>Learn more</a>.",Url.Action("Index","Billing"))}
          }
        };
        return Content(JsonConvert.SerializeObject(errResult), "application/json");
      }
      ListHelper.InitializeAreas(CurrentUser);
      ListHelper.InitializeMetrics(CurrentUser);
      ListHelper.InitializeMetricDataSources(CompanyId);
      int[] myMetricIds = ListHelper.GetMetrics().Where(m => m.DataSourceId.HasValue).Select(m => m.Id).ToArray();

      var results = await _metricManager.GetValuesFromDataSource(CompanyId, year, month, myMetricIds);

      return Content(JsonConvert.SerializeObject(results, Formatting.None,
        new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" }), "application/json");
    }

  }
}
