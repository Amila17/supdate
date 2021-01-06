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
  public class MetricDataPointRepository : CrudRepository<MetricDataPoint>, IMetricDataPointRepository
  {

    public void SaveMetricDataPoint(int companyId,  MetricDataPoint metricDataPoint)
    {
      try
      {
        OpenConnection();

        Connection.Execute("MetricDataPointSave", new { companyId, metricId = metricDataPoint.MetricId, date = metricDataPoint.Date, actual = metricDataPoint.Actual, target=metricDataPoint.Target }, commandType: CommandType.StoredProcedure);
      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
