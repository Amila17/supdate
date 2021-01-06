using System;
using System.Collections.Generic;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public interface IReportEmailRepository : ICrudRepository<ReportEmail>
  {
    ReportEmail GetByEmailAddress(int companyId, int reportId, string emailAddress);
    ReportEmail GetByGuid(Guid reportEmailGuid);
  }
}
