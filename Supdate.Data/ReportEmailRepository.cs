using System;
using System.Data;
using System.Linq;
using Dapper;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public class ReportEmailRepository : CrudRepository<ReportEmail>, IReportEmailRepository
  {
    public ReportEmail GetByEmailAddress(int companyId, int reportId, string emailAddress)
    {
      try
      {
        OpenConnection();

        return Connection.Query<ReportEmail>("ReportEmailDetailsGetByAddress",
          new { CompanyId = companyId, ReportId = reportId, EmailAddress = emailAddress },
          commandType: CommandType.StoredProcedure).FirstOrDefault();
      }
      finally
      {
        CloseConnection();
      }
    }

    public ReportEmail GetByGuid(Guid reportEmailGuid)
    {
      try
      {
        OpenConnection();

        return Connection.Query<ReportEmail>("ReportEmailGetByGuid",
          new { reportEmailGuid },
          commandType: CommandType.StoredProcedure).FirstOrDefault();
      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
