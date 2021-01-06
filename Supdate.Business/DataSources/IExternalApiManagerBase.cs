using System;
using System.Collections.Generic;
using System.Net.Http;
using Supdate.Model;

namespace Supdate.Business.DataSources
{
  public interface IExternalApiManagerBase
  {
    HttpClient GetHttpClient(ExternalApi externalApi, ExternalApiAuth externalApiAuth);

    IEnumerable<MetricDataPoint> CreateDataPointsForMatches(IEnumerable<MetricDataPoint> metricDataPoints, DateTime date, MetricDataSource dataSource, double value);
  }
}
