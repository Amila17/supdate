using System;
using System.Collections.Generic;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public interface IMetricRepository : ICrudRepository<Metric>
  {
    void SaveDisplayOrder(int companyId, IList<EntityDisplayOrder> displayOrders);
    Metric GetMetric(int companyId, Guid metricGuid);
    void DeleteMetric(int companyId, Guid metricGuid);
    IEnumerable<GraphPoint> GraphData(int companyId, Guid? reportUniqueId = null);
    IEnumerable<MetricView> GetMetricViews(int companyId, DateTime? reportDate);
    void UpdateMetricMetadata(int metricId);
    IEnumerable<MetricDataPoint> GetDataPointsForPeriod(int companyId, int year, int? month);
  }
}
