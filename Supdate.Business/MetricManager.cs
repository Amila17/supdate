using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Supdate.Business.DataSources;
using Supdate.Data;
using Supdate.Model;

namespace Supdate.Business
{
  public class MetricManager : Manager<Metric>, IMetricManager
  {
    private readonly IMetricRepository _metricRepository;
    private readonly IMetricDataPointRepository _metricDataPointRepository;
    private readonly IChartMogulApiManager _chartMogulApiManager;
    private readonly IGoogleAnalyticsApiManager _googleAnalyticsApiManager;
    private readonly IExternalApiAuthManager _externalApiAuthManager;
    private readonly IGoogleAuthorizer _googleAuthorizer;
    private readonly ICompanyManager _companyManager;

    public MetricManager(IMetricRepository metricRepository, IMetricDataPointRepository metricDataPointRepository,
      IChartMogulApiManager chartMogulApiManager, IGoogleAnalyticsApiManager googleAnalyticsApiManager, IExternalApiAuthManager externalApiAuthManager,
      IGoogleAuthorizer googleAuthorizer, ICompanyManager companyManager)
      : base(metricRepository)
    {
      _metricRepository = metricRepository;
      _metricDataPointRepository = metricDataPointRepository;
      _chartMogulApiManager = chartMogulApiManager;
      _googleAnalyticsApiManager = googleAnalyticsApiManager;
      _externalApiAuthManager = externalApiAuthManager;
      _googleAuthorizer = googleAuthorizer;
      _companyManager = companyManager;
    }

    public override Metric Create(Metric model)
    {
      using (var scope = new TransactionScope())
      {
        var newMetric = Repository.Create(model);
        scope.Complete();

        return newMetric;
      }
    }

    public Metric GetMetric(int companyId, Guid uniqueId)
    {
      return _metricRepository.GetMetric(companyId, uniqueId);
    }

    public void DeleteMetric(int companyId, Guid uniqueId)
    {
      _metricRepository.DeleteMetric(companyId,uniqueId);
    }

    public override Metric Update(Metric model)
    {
      using (var scope = new TransactionScope())
      {
        var updatedMetric = Repository.Update(model);
        scope.Complete();

        return updatedMetric;
      }
    }

    public void SaveDisplayOrder(int companyId, IList<EntityDisplayOrder> displayOrders)
    {
      _metricRepository.SaveDisplayOrder(companyId,displayOrders);
    }

    public IEnumerable<MetricGraph> GraphData(int companyId, Guid? reportUniqueId = null)
    {
      var metricGraphs = new List<MetricGraph>();

      // Get the graph data for multiple metrics
      var graphData =  _metricRepository.GraphData(companyId, reportUniqueId);

      // loop through each unique metric in graph data and create a MetricGraph for each
      var graphPoints = graphData as IList<GraphPoint> ?? graphData.ToList();

      foreach (var metricGuid in graphPoints.Select(m => m.UniqueId).Distinct().ToList())
      {
        var metricGraph = new MetricGraph{UniqueId =  metricGuid};

        // Get just the data points for this metric, ordered by date
        var dataPoints = graphPoints.Where(m => m.UniqueId == metricGuid).OrderBy(m => m.Date).ToList();

        // We need to know which date is the current date
        var currentMonthDataPoint = dataPoints.FirstOrDefault(m => m.IsCurrentDate);

        // If there isn't a data point flagged as the current date, use the max date provided with a value
        var firstPoint = dataPoints.Where(m => m.Actual != 0 && m.Actual != null && m.Date <= DateTime.UtcNow).OrderByDescending(m => m.Date).FirstOrDefault();
        if (firstPoint != null)
        {
          var currentDate = currentMonthDataPoint != null
            ? currentMonthDataPoint.Date
            : firstPoint.Date;

          // The thumbnail graph should only run to the current date
          metricGraph.ThumbnailGraph = dataPoints.Where(m => m.Date <= currentDate).OrderByDescending(m => m.Date).Take(12).OrderBy(m => m.Date).ToList();
        }

        // Provide the full dataset for the full graph
        metricGraph.FullGraph = dataPoints;

        // Add this graph to our collection
        metricGraphs.Add(metricGraph);
      }

      // Return all of our lovely graphs
      return metricGraphs;
    }

    public void SaveMetricDataPoints(int companyId, List<MetricDataPoint> metricDataPoints, int[] myMetricIds)
    {

      // Filter the list to ensure we're only updating data points for metrics the user has access to
      foreach (var metricDataPoint in metricDataPoints.Where(m => myMetricIds.Contains(m.MetricId)).ToList())
      {
        if (metricDataPoint.Actual.HasValue || metricDataPoint.Target.HasValue)
        {
            _metricDataPointRepository.SaveMetricDataPoint(companyId, metricDataPoint);
        }
        else if (metricDataPoint.Id != 0)
        {
          // Security Check to make sure metric Id hasn't been changed
          var metricDataPintToDelete = _metricDataPointRepository.Get(metricDataPoint.Id);

          if (metricDataPintToDelete.MetricId == metricDataPoint.MetricId) {
            _metricDataPointRepository.Delete(metricDataPoint.Id);
          }
        }
      }

      // Run for each distinct MetricID in the collection, not for EVERY entry in the collection
      foreach (var metricId in metricDataPoints.Select(m => m.MetricId).Distinct())
      {
        UpdateMetricMetadata(metricId);
      }
    }

    private void UpdateMetricMetadata(int metricId)
    {
      _metricRepository.UpdateMetricMetadata(metricId);
    }

    public IEnumerable<MetricView> GetMetricViews(int companyId, DateTime? reportDate = null, LiteUser currentUser = null)
    {
      var results = _metricRepository.GetMetricViews(companyId, reportDate);
      if (results != null && currentUser != null && !currentUser.IsCompanyAdmin)
      {
        results = results.Where(m => m.AreaId.HasValue && currentUser.AccessibleAreaIds.Contains(m.AreaId.Value)).ToList();
      }
      return results;
    }

    public IEnumerable<MetricDataPoint> GetDataPointsForPeriod(int companyId, int year, int? month, int[] myMetricIds)
    {
      var results = _metricRepository.GetDataPointsForPeriod(companyId, year, month);

      // ensure we only return data for metrics the user is allowed to access
      return results.Where(m => myMetricIds.Contains(m.MetricId)).ToList();
    }

    public async Task<MetricDataImport> GetValuesFromDataSource(int companyId, int year, int? month, int[] myMetricIds)
    {
      var metricDataImport = new MetricDataImport { success = true };

      // First we get existing MetricDataPoints to cover the period specified
      var metricDataPoints = GetDataPointsForPeriod(companyId, year, month, myMetricIds).ToList();

      // Remove any MetricDataPoints that are in the future
      metricDataPoints.RemoveAll(m=> m.Date > DateTime.Today);


      var googleExternalApiAuth = _externalApiAuthManager.GetExternalApiAuth(companyId, ExternalApi.GoogleAnalytics.Id);
      if (googleExternalApiAuth != null)
      {
        // Get details needed for Google authorization
        var company = _companyManager.Get(companyId);
        var auth = _googleAuthorizer.Authorize(company.UniqueId, googleExternalApiAuth, "", CancellationToken.None);
        // Get Google data
        var googleAnalyticsImport = await _googleAnalyticsApiManager.GetData(companyId, _externalApiAuthManager.GetExternalApiAuth(companyId, 2), auth.Result, metricDataPoints);
        // Add results to local objecy
        metricDataImport.results.AddRange(googleAnalyticsImport.results);
        metricDataImport.errors.AddRange(googleAnalyticsImport.errors);
      }

      // Get ChartMogul data
      var chartMogulImport = await _chartMogulApiManager.GetData(companyId, _externalApiAuthManager.GetExternalApiAuth(companyId,ExternalApi.ChartMogul.Id), metricDataPoints);
      // Add results to the local object
      metricDataImport.results.AddRange(chartMogulImport.results);
      metricDataImport.errors.AddRange(chartMogulImport.errors);

      return metricDataImport;
    }
  }
}
