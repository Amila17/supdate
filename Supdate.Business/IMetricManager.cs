using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supdate.Model;

namespace Supdate.Business
{
  public interface IMetricManager : IManager<Metric>
  {
    void SaveDisplayOrder(int companyId, IList<EntityDisplayOrder> displayOrders);
    Metric GetMetric(int companyId, Guid uniqueId);
    void DeleteMetric(int companyId, Guid uniqueId);
    IEnumerable<MetricGraph> GraphData(int companyId, Guid? reportUniqueId = null);
    void SaveMetricDataPoints(int companyId, List<MetricDataPoint> metricDataPoints, int[] myMetricIds);
    IEnumerable<MetricView> GetMetricViews(int companyId, DateTime? reportDate = null, LiteUser currentUser = null);
    IEnumerable<MetricDataPoint> GetDataPointsForPeriod(int companyId, int year, int? month, int[] myMetricIds);
    Task<MetricDataImport> GetValuesFromDataSource(int companyId, int year, int? month, int[] myMetricIds);
  }
}
