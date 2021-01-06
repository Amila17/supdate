using System;
using System.Data;
using System.Linq;
using Dapper;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public class ReportAreaRepository : CrudRepository<ReportArea>, IReportAreaRepository
  {
    public ReportArea GetReportArea(int companyId, Guid areaUniqueId, DateTime reportDate)
    {
      try
      {
        OpenConnection();

        var results = Connection.QueryMultiple("ReportAreaGet",
          new { CompanyId = companyId, AreaUniqueId = areaUniqueId, ReportDate = reportDate },
          commandType: CommandType.StoredProcedure);

        var reportArea = results.Read<ReportArea>().Single();

        if (reportArea != null)
        {
          reportArea.MetricList = results.Read<MetricView>().ToList();
          reportArea.ReportGoalList = results.Read<ReportGoalView>().ToList();
          reportArea.Attachments = results.Read<ReportAttachment>().ToList();
        }

        return reportArea;
      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
