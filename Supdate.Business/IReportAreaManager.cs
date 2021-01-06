using System;
using Supdate.Model;

namespace Supdate.Business
{
  public interface IReportAreaManager : IManager<ReportArea>
  {
    ReportArea GetReportArea(int companyId, Guid areaUniqueId, DateTime reportDate);
  }
}
