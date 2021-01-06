using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Supdate.Model;

namespace Supdate.Business.DataSources
{
  public abstract class ExternalApiManagerBase : IExternalApiManagerBase
  {
    public HttpClient GetHttpClient(ExternalApi externalApi, ExternalApiAuth externalApiAuth)
    {
      var client = new HttpClient
      {
        BaseAddress = new Uri(externalApi.BaseUrl)
      };
      var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", externalApiAuth.Token, externalApiAuth.Key));

      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

      return client;
    }

    public IEnumerable<MetricDataPoint> CreateDataPointsForMatches(IEnumerable<MetricDataPoint> metricDataPoints, DateTime date, MetricDataSource dataSource, double value)
    {
      value = Math.Round(value, 2);
      var resultantDataPoints = new List<MetricDataPoint>();

      var dataPoints = metricDataPoints as IList<MetricDataPoint> ?? metricDataPoints.ToList();

      if (dataPoints.Any(m => m.Date == date && m.DataSourceId == (int)dataSource))
      {
        // find all DataPoints that are for the specified date and set to the specified DataSource
        foreach (var metricDataPoint in dataPoints.Where(m => m.Date == date && m.DataSourceId == (int)dataSource))
        {
          resultantDataPoints.Add(new MetricDataPoint { Date = date, MetricId = metricDataPoint.MetricId, Actual = value });
        }
      }

      return resultantDataPoints;
    }
  }
}
