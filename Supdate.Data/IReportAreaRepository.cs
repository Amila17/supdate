using System;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public interface IReportAreaRepository: ICrudRepository<ReportArea>
  {
    ReportArea GetReportArea(int companyId, Guid areaUniqueId, DateTime reportDate);
  }
}
