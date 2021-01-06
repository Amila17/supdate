using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Supdate.Business;
using Supdate.Business.DataSources;
using Supdate.Model;
using Supdate.Util;

namespace Supdate.Web.App.Models
{
  public class ListHelper
  {
    private Dictionary<int, string> _reportTypeMap;
    private Dictionary<int, string> _goalStatusMap;
    private Dictionary<int, string> _areaMap;
    private Dictionary<int, string> _graphTypesMap;
    private IEnumerable<Goal> _goals;
    private IEnumerable<Metric> _metrics;
    private IEnumerable<Area> _areas;
    private IList<MetricDataSource> _metricDataSources;

    private readonly IAreaManager _areaManager;
    private readonly IGoalManager _goalManager;
    private readonly IMetricManager _metricManager;
    private readonly IExternalApiAuthManager _externalApiAuthManager;

    public ListHelper(IAreaManager areaManager, IGoalManager goalManager, IMetricManager metricManager, IExternalApiAuthManager externalApiAuthManager)
    {
      _areaManager = areaManager;
      _goalManager = goalManager;
      _metricManager = metricManager;
      _externalApiAuthManager = externalApiAuthManager;
    }

    public void InitializeReportTypeList()
    {
      _reportTypeMap = new Dictionary<int, string>
      {
        { (int) ReportType.Weekly, ReportType.Weekly.DisplayName() },
        { (int) ReportType.Monthly, ReportType.Monthly.DisplayName() },
        { (int) ReportType.Quarterly, ReportType.Quarterly.DisplayName() },
      };
    }

    public List<SelectListItem> ReportTypeList
    {
      get
      {
        return
          _reportTypeMap.Select(mapItem => new SelectListItem { Text = mapItem.Value, Value = mapItem.Key.ToString(CultureInfo.InvariantCulture) })
            .ToList();
      }
    }

    public void InitializeGoalStatusList()
    {
      _goalStatusMap = new Dictionary<int, string>
      {
        { (int) GoalStatus.NotStarted, GoalStatus.NotStarted.DisplayName() },
        { (int) GoalStatus.InProgressOnSchedule, GoalStatus.InProgressOnSchedule.DisplayName() },
        { (int) GoalStatus.InProgressDelayed, GoalStatus.InProgressDelayed.DisplayName() },
        { (int) GoalStatus.Completed, GoalStatus.Completed.DisplayName() },
        { (int) GoalStatus.Cancelled, GoalStatus.Cancelled.DisplayName() }
      };
    }

    public List<SelectListItem> GoalStatusList
    {
      get
      {
        return
          _goalStatusMap.Select(mapItem => new SelectListItem { Text = mapItem.Value, Value = mapItem.Key.ToString(CultureInfo.InvariantCulture) })
            .ToList();
      }
    }

    public void InitializeAreas(LiteUser currentUser)
    {
      if (_areaMap == null)
      {
        _areaMap = new Dictionary<int, string>();

        if (AllowUnassignedGoalsAndMetrics(currentUser))
        {
          // add default element
          _areaMap.Add(-1,"-none-");
        }

          // Fetch from the database and initialize the map.
        var allAreas = _areaManager.GetList(new { CompanyId = currentUser.CompanyId }).OrderBy(x => x.DisplayOrder).ThenBy(i => i.Id);

        _areas = allAreas.Where(a => currentUser.AccessibleAreaIds.Contains(a.Id));

        foreach (var area in _areas)
        {
          _areaMap.Add(area.Id, area.Name);
        }
      }
    }

    public IEnumerable<Area> GetAreas()
    {
      return _areas;
    }

    public IEnumerable<MetricDataSource> GetMetricDataSources()
    {
      return _metricDataSources;
    }

    public string GetAreaName(int id)
    {
      return _areaMap[id];
    }
    public List<SelectListItem> AreaList
    {
      get
      {
        return
          _areaMap.Select(a => new SelectListItem { Text = a.Value, Value = a.Key.ToString() })
            .ToList();
      }
    }
    public void InitializeGraphTypesList()
    {
      _graphTypesMap = new Dictionary<int, string>
      {
        { (int) GraphType.NoGraph, GraphType.NoGraph.DisplayName() },
        { (int) GraphType.LineChart, GraphType.LineChart.DisplayName() },
        { (int) GraphType.BarGraph, GraphType.BarGraph.DisplayName() }
      };
    }

    public List<SelectListItem> GraphTypesList
    {
      get
      {
        return
          _graphTypesMap.Select(mapItem => new SelectListItem { Text = mapItem.Value, Value = mapItem.Key.ToString() })
            .ToList();
      }
    }

    public void InitializeGoals(LiteUser currentUser)
    {
      if (_goals == null)
      {
        // Fetch from the database and initialize the map.
        var allgoals = _goalManager.GetList(new { CompanyId = currentUser.CompanyId });
        _goals = currentUser.IsCompanyAdmin ? allgoals : allgoals.Where(g => g.AreaId.HasValue && currentUser.AccessibleAreaIds.Contains(g.AreaId.Value));
      }
    }

    public IEnumerable<Goal> GetGoals()
    {
      return _goals;
    }

    public void InitializeMetrics(LiteUser currentUser)
    {
      if (_metrics == null)
      {
        // Fetch from the database and initialize the map.
        var allMetrics = _metricManager.GetList(new { CompanyId = currentUser.CompanyId });
        _metrics = currentUser.IsCompanyAdmin ? allMetrics : allMetrics.Where(m => m.AreaId.HasValue && currentUser.AccessibleAreaIds.Contains(m.AreaId.Value));
      }
    }

    public void InitializeMetricDataSources(int companyId)
    {
      if (_metricDataSources == null)
      {
        var metricDataSources = new List<MetricDataSource>();

        // Get each API that is integrated for this company
        foreach (var api in _externalApiAuthManager.GetExternalApiAuths(companyId).Select(a => a.ExternalApi).Distinct())
        {
          var externalApimanager = _externalApiAuthManager.GetApiManager(api.Id);
          metricDataSources.AddRange(externalApimanager.MetricDataSources());
        }
        _metricDataSources = metricDataSources;
      }
    }
    public List<SelectListItem> MetricDataSourceList
    {
      get
      {
        var listItems =
          _metricDataSources.Select(a => new SelectListItem { Text = a.DisplayName(), Value = ((int)a).ToString() })
            .ToList();

        if (listItems.Count > 0)  listItems.Insert(0,new SelectListItem{Text = "-none-", Value = "-1"});

        return listItems;
      }
    }
    public IEnumerable<Metric> GetMetrics()
    {
      return _metrics;
    }

    private bool AllowUnassignedGoalsAndMetrics(LiteUser currentUser)
    {
      return currentUser.IsCompanyAdmin;
    }

    public int[] MetricIds()
    {
      return GetMetrics().Select(m => m.Id).ToArray();
    }
    public int GoalCountForArea(int areaId)
    {
      return _goals.Count(g => g.AreaId == areaId);
    }
    public int MetricCountForArea(int areaId)
    {
      return _metrics.Count(g => g.AreaId == areaId);
    }

    public string AreaIdsToString(int[] areaIds, int maxChars)
    {
      int areaListChars = 0;
      int areasListed = 0;
      StringBuilder sb = new StringBuilder();
      for (var i = 0; i < areaIds.Count(); i++)
      {
        var areaName = GetAreaName(areaIds[i]);
        areaListChars = areaListChars + areaName.Length;
        if (areaListChars < maxChars)
        {
          areasListed++;
          if (i > 0)
          {
            sb.Append(", ");
          }
          sb.Append(areaName);
        }
      }
      if (areasListed < areaIds.Count())
      {
        sb.Append("<em class='text-muted small'> +" + (areaIds.Count() - areasListed) + " more.</em>");
      }
      return sb.ToString();
    }
  }
}
