using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Supdate.Data.Base;
using Supdate.Model;
using Dapper;
using Supdate.Util;

namespace Supdate.Data
{
  public class MetricRepository : CrudRepository<Metric>, IMetricRepository
  {
    public override Metric Create(Metric metric)
    {
      try
      {
        OpenConnection();

        metric.DisplayOrder = 999;
        metric.Id = Connection.Insert<int>(metric);

        return metric;
      }
      finally
      {
        CloseConnection();
      }
    }

    public Metric GetMetric(int companyId, Guid metricGuid)
    {
      try
      {
        OpenConnection();
        var results = Connection.QueryMultiple("MetricGet", new { companyId, metricGuid }, commandType: CommandType.StoredProcedure);
        return results.Read<Metric>().FirstOrDefault();
      }
      finally
      {
        CloseConnection();
      }
    }

    public void DeleteMetric(int companyId, Guid metricGuid)
    {
      try
      {
        var metric = GetMetric(companyId, metricGuid);

        Delete(metric.Id);
      }
      finally
      {
        CloseConnection();
      }
    }

    public override Metric Update(Metric model)
    {
      try
      {
        var modelCheck = GetMetric(model.CompanyId, model.UniqueId);
        if (modelCheck.Id != model.Id)
        {
          return model;
        }

        OpenConnection();
        Connection.Update(model);

        return model;
      }
      finally
      {
        CloseConnection();
      }
    }

    public override bool Delete(int id)
    {
      try
      {
        OpenConnection();

        return Connection.Execute("MetricDeleteById", new { metricId = id }, commandType: CommandType.StoredProcedure) > 0;
      }
      finally
      {
        CloseConnection();
      }
    }

    public void SaveDisplayOrder(int companyId, IList<EntityDisplayOrder> displayOrders)
    {
      try
      {
        var orderData = ConversionUtil.EntityDisplayOrderToDataTable(displayOrders);
        OpenConnection();
        Connection.Execute("MetricsSaveOrder", new { companyId = companyId, OrderData = orderData }, commandType: CommandType.StoredProcedure);
      }
      finally
      {
        CloseConnection();
      }
    }

    /// <summary>
    /// Returns data points for multiple metrics
    /// </summary>
    public IEnumerable<GraphPoint> GraphData(int companyId, Guid? reportUniqueId = null)
    {
      try
      {
        OpenConnection();
        return  Connection.Query<GraphPoint>("MetricsGraphData", new { companyId, reportUniqueId }, commandType: CommandType.StoredProcedure).ToList();

      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<MetricView> GetMetricViews(int companyId, DateTime? reportDate)
    {
      try
      {
        OpenConnection();

        var results = Connection.QueryMultiple("MetricViewsGet",
          new { CompanyId = companyId, ReportDate = reportDate },
          commandType: CommandType.StoredProcedure);

        return results.Read<MetricView>().ToList();
      }
      finally
      {
        CloseConnection();
      }
    }

    public void UpdateMetricMetadata(int metricId)
    {
      try
      {
        OpenConnection();

        Connection.Execute("MetricUpdateMetadata", new { metricId }, commandType: CommandType.StoredProcedure);
      }
      finally
      {
        CloseConnection();
      }
    }

    public IEnumerable<MetricDataPoint> GetDataPointsForPeriod(int companyId, int year, int? month)
    {
      try
      {
        OpenConnection();

        var results = Connection.QueryMultiple("MetricDataPointsGetForPeriod",
          new { CompanyId = companyId, Year = year, Month = month },
          commandType: CommandType.StoredProcedure);

        return results.Read<MetricDataPoint>().ToList();

      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
