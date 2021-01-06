using System;
using Supdate.Data;
using Supdate.Model;

namespace Supdate.Business
{
  public class ReportAreaManager : Manager<ReportArea>, IReportAreaManager
  {
    private readonly IReportAreaRepository _reportAreaRepository;

    public ReportAreaManager(IReportAreaRepository reportAreaRepository)
      : base(reportAreaRepository)
    {
      _reportAreaRepository = reportAreaRepository;
    }

    public ReportArea GetReportArea(int companyId, Guid areaUniqueId, DateTime reportDate)
    {
      return _reportAreaRepository.GetReportArea(companyId, areaUniqueId, reportDate);
    }
  }
}
