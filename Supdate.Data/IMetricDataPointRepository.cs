using System;
using System.Collections.Generic;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public interface IMetricDataPointRepository : ICrudRepository<MetricDataPoint>
  {
    void SaveMetricDataPoint(int companyId, MetricDataPoint metricDataPoint);
  }
}
