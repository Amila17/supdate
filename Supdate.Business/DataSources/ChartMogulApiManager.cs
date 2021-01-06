using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Web;
using Newtonsoft.Json;
using Supdate.Model;
using Supdate.Model.Exceptions;

namespace Supdate.Business.DataSources
{
  public class ChartMogulApiManager : ExternalApiManagerBase, IChartMogulApiManager
  {
    private readonly ExternalApi _externalApi = ExternalApi.ChartMogul;
    private static string _dataUrl = "metrics/all?start-date={0}&end-date={1}";

    public IList<MetricDataSource> MetricDataSources()
    {
      var metricDataSources = new List<MetricDataSource>();
      metricDataSources.Add((MetricDataSource)1);
      metricDataSources.Add((MetricDataSource)2);
      metricDataSources.Add((MetricDataSource)3);
      metricDataSources.Add((MetricDataSource)4);
      metricDataSources.Add((MetricDataSource)5);
      metricDataSources.Add((MetricDataSource)6);
      metricDataSources.Add((MetricDataSource)7);
      metricDataSources.Add((MetricDataSource)8);

      return metricDataSources;
    }

    public async Task<bool> TestCredentials(string token, string key, int externalApiId)
    {
      var externalApiAuth = new ExternalApiAuth { Token = token, Key = key };
      var client = GetHttpClient(_externalApi, externalApiAuth);

      HttpResponseMessage response = await client.GetAsync(_externalApi.TestUrl);
      if (response.IsSuccessStatusCode)
      {
        return true;
      }

      return false;
    }

    public Task<MetricDataImport> GetData(int companyId, ExternalApiAuth externalApiAuth, AuthorizationCodeWebApp.AuthResult authResult, IEnumerable<MetricDataPoint> metricDataPoints)
    {
      throw new BusinessException("Not Implemented");
    }

    public async Task<MetricDataImport> GetData(int companyId, ExternalApiAuth externalApiAuth, IEnumerable<MetricDataPoint> metricDataPoints)
    {
      var metricDataImport = new MetricDataImport { success = true };
      if (externalApiAuth == null)
      {
        // user is not connected to this API
        return metricDataImport;
      }

      var resultantDataPoints = new List<MetricDataPoint>();
      var dataPoints = metricDataPoints as IList<MetricDataPoint> ?? metricDataPoints.ToList();
      var dateList = dataPoints.Select(d => d.Date).Distinct();

      try
      {
        // Loop for each distinct date provided in the MetricDataPoints
        foreach (var date in dateList)
        {
          // We want data for the last day of the month specified
          var dataDate = date.AddMonths(1).AddDays(-1);

          // Unless that puts us in the future, in which case get today's data
          if (dataDate > DateTime.Today)
          {
            dataDate = DateTime.Today;
          }

          var metricsChartMogul = dataPoints.Where(m => m.DataSourceId.HasValue && m.DataSourceId > 0 && m.DataSourceId < 9);
          if (metricsChartMogul.Any())
          {
            // Setup objects for API call
            var client = GetHttpClient(_externalApi, externalApiAuth);

            // Call API to get all ChartMogul numbers. They can't take a range of a single day so we need to ask for two even though we only need one : /
            var response = await client.GetAsync(string.Format(_dataUrl, dataDate.AddDays(-1).ToString("yyyy-MM-dd"), dataDate.ToString("yyyy-MM-dd")));

            if (response.IsSuccessStatusCode)
            {
              // Process resultant JSON
              string json = await response.Content.ReadAsStringAsync();
              ChartMogulResponse results = JsonConvert.DeserializeObject<ChartMogulResponse>(json);
              resultantDataPoints.AddRange(ParseResults(results, dataDate, date, dataPoints));
            }
            else
            {
              var errMsg = string.Format("{0}: {1}", _externalApi.Name, response.StatusCode);

              // Don't add the same error multiple times
              if (!metricDataImport.errors.Any(e => e.ErrorMessage == errMsg))
              {
                var err = new JsonError { ErrorMessage = errMsg };
                metricDataImport.errors.Add(err);
              }
            }
          }
        }
      }
      catch (Exception e)
      {
        metricDataImport.errors.Add(new JsonError { ErrorMessage = string.Format("{0}: {1}", _externalApi.Name, e.Message) });
      }

      metricDataImport.results = resultantDataPoints;

      return metricDataImport;
    }

    /// <summary>
    /// Takes the results from ChartMoguls /metrics/all API call
    /// and returns new data points for any values found that match the submitted date/metrics
    /// </summary>
    private IEnumerable<MetricDataPoint> ParseResults(ChartMogulResponse results, DateTime dataDate, DateTime date, IEnumerable<MetricDataPoint> metricDataPoints)
    {
      var resultantDataPoints = new List<MetricDataPoint>();

      // Look for entry for the last day of previous month
      if (results.Entries.Select(e => e.Date == dataDate.ToString("yyyy-MM-dd")).Any())
      {
        var result = results.Entries.FirstOrDefault(e => e.Date == dataDate.ToString("yyyy-MM-dd"));

        // Add new MetricDataPoints for any Metrics that are mapped to any of these DataSources
        var dataPoints = metricDataPoints as IList<MetricDataPoint> ?? metricDataPoints.ToList();

        if (result != null)
        {
          resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.ChartMogulMrr, result.Mrr));
          resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.ChartMogulArr, result.Arr / 100));
          resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.ChartMogulArpa, result.Arpa / 100));
          resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.ChartMogulAsp, result.Asp / 100));
          resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.ChartMogulCustomerCount, result.Customers));
          resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.ChartMogulCcr, result.CustomerChurnRate));
          resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.ChartMogulMrrChurnRate, result.MrrChurnRate));
          resultantDataPoints.AddRange(CreateDataPointsForMatches(dataPoints, date, MetricDataSource.ChartMogulLtv, result.Ltv / 100));
        }
      }

      return resultantDataPoints;
    }
  }
}
