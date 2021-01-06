using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Services;
using Supdate.Model;
using Supdate.Model.Exceptions;

namespace Supdate.Business.DataSources
{
  public class GoogleAnalyticsApiManager : ExternalApiManagerBase, IGoogleAnalyticsApiManager
  {
    public IList<MetricDataSource> MetricDataSources()
    {
      var metricDataSources = new List<MetricDataSource>();
      metricDataSources.Add((MetricDataSource)9);
      metricDataSources.Add((MetricDataSource)10);
      metricDataSources.Add((MetricDataSource)11);
      metricDataSources.Add((MetricDataSource)12);
      metricDataSources.Add((MetricDataSource)13);
      metricDataSources.Add((MetricDataSource)14);
      metricDataSources.Add((MetricDataSource)15);

      return metricDataSources;
    }

    public async Task<bool> TestCredentials(string token, string key, int externalApiId)
    {
      throw new BusinessException("Not Implemented");
    }

    public async Task<MetricDataImport> GetData(int companyId, ExternalApiAuth externalApiAuth, IEnumerable<MetricDataPoint> metricDataPoints)
    {
      throw new BusinessException("Not Implemented");
    }

    public async Task<MetricDataImport> GetData(int companyId, ExternalApiAuth externalApiAuth, AuthorizationCodeWebApp.AuthResult authResult, IEnumerable<MetricDataPoint> metricDataPoints)
    {
      var metricDataImport = new MetricDataImport { success = true };
      if (externalApiAuth == null)
      {
        // user is not connected to this API
        return metricDataImport;
      }

      if (authResult.Credential == null)
      {
        // should be connected, but Google says we're not
        metricDataImport.errors.Add(new JsonError { ErrorMessage = "Google Analytics authorization revoked - unable to get data" });

        return metricDataImport;
      }

      metricDataImport.results = GAReports(externalApiAuth, authResult, metricDataPoints);

      return metricDataImport;
    }

    private List<MetricDataPoint> GAReports(ExternalApiAuth externalApiAuth, AuthorizationCodeWebApp.AuthResult authResult, IEnumerable<MetricDataPoint> metricDataPoints)
    {
      var resultantDataPoints = new List<MetricDataPoint>();

      if (metricDataPoints != null)
      {
        // Initiate GA Reporting Service
        var service = new AnalyticsReportingService(new BaseClientService.Initializer
        {
          HttpClientInitializer = authResult.Credential,
          ApplicationName = "Supdate"
        });

        // Get a list of each date we need data for
        var dataPoints = metricDataPoints as IList<MetricDataPoint> ?? metricDataPoints.ToList();
        var dateList = dataPoints.Select(d => d.Date).Distinct();
        var reportMetrics = GAReportMetrics();

        foreach (var date in dateList)
        {
          // Get a report request
          var reportsRequest = PrepareGAReportRequest(date, reportMetrics, externalApiAuth.ConfigData);

          // Execute the request
          var reportResponse = service.Reports.BatchGet(reportsRequest).Execute();

          // Extract the resultant report data
          resultantDataPoints.AddRange(ParseResults(date, reportResponse.Reports[0], dataPoints));
        }
      }

      // Return the results of all report requests
      return resultantDataPoints;
    }

    private string ExtractReportValueFor(string metricName, Google.Apis.AnalyticsReporting.v4.Data.Report report)
    {
      var header = report.ColumnHeader.MetricHeader.MetricHeaderEntries.FirstOrDefault(h => h.Name == metricName);
      var i = report.ColumnHeader.MetricHeader.MetricHeaderEntries.IndexOf(header);

      return report.Data.Totals[0].Values[i];
    }

    private GetReportsRequest PrepareGAReportRequest(DateTime date, IList<Google.Apis.AnalyticsReporting.v4.Data.Metric> metrics, string viewId)
    {
      // We want data up to the last day of the month specified
      var endDate = date.AddMonths(1).AddDays(-1);
      var dateRanges = new List<DateRange> { new DateRange { StartDate = date.ToString("yyyy-MM-dd"), EndDate = endDate.ToString("yyyy-MM-dd") } };

      // Create a single report request
      var reportRequest = new ReportRequest
      {
        Metrics = metrics,
        DateRanges = dateRanges,
        ViewId = viewId
      };

      // Prepare the request to be sent to the API
      return new GetReportsRequest { ReportRequests = new List<ReportRequest> { reportRequest } };
    }

    private List<MetricDataPoint> ParseResults(DateTime date, Google.Apis.AnalyticsReporting.v4.Data.Report report, IEnumerable<MetricDataPoint> metricDataPoints)
    {
      var resultantDataPoints = new List<MetricDataPoint>();

      var dataPoints = metricDataPoints as IList<MetricDataPoint> ?? metricDataPoints.ToList();

      resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.GASessions, int.Parse(ExtractReportValueFor("Sessions", report))));
      resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.GAUsers, int.Parse(ExtractReportValueFor("Users", report))));
      resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.GAPageViews, int.Parse(ExtractReportValueFor("PageViews", report))));
      resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.GAUniquePageViews, int.Parse(ExtractReportValueFor("UniquePageViews", report))));
      resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.GAGoalCompletions, int.Parse(ExtractReportValueFor("GoalCompletions", report))));
      resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.GAPageViewsPerSessions, double.Parse(ExtractReportValueFor("PageViewsPerSession", report))));
      resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.GAGoalConversion, double.Parse(ExtractReportValueFor("GoalConversions", report))));

      return resultantDataPoints;
    }

    private IList<Google.Apis.AnalyticsReporting.v4.Data.Metric> GAReportMetrics()
    {
      var metrics = new List<Google.Apis.AnalyticsReporting.v4.Data.Metric>();

      metrics.Add(new Google.Apis.AnalyticsReporting.v4.Data.Metric { Expression = "ga:sessions", Alias = "Sessions" });
      metrics.Add(new Google.Apis.AnalyticsReporting.v4.Data.Metric { Expression = "ga:users", Alias = "Users" });
      metrics.Add(new Google.Apis.AnalyticsReporting.v4.Data.Metric { Expression = "ga:pageviews", Alias = "PageViews" });
      metrics.Add(new Google.Apis.AnalyticsReporting.v4.Data.Metric { Expression = "ga:uniquePageviews", Alias = "UniquePageViews" });
      metrics.Add(new Google.Apis.AnalyticsReporting.v4.Data.Metric { Expression = "ga:goalCompletionsAll", Alias = "GoalCompletions" });
      metrics.Add(new Google.Apis.AnalyticsReporting.v4.Data.Metric { Expression = "ga:pageviewsPerSession", Alias = "PageViewsPerSession" });
      metrics.Add(new Google.Apis.AnalyticsReporting.v4.Data.Metric { Expression = "ga:goalConversionRateAll", Alias = "GoalConversions" });

      return metrics;
    }
  }

  // https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth#web-applications-aspnet-mvc

}
