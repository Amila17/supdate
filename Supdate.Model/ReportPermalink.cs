using System;

namespace Supdate.Model
{
  public class ReportPermalink
  {
    public Guid UniqueId { get; set; }

    public DateTime Date { get; set; }

    public int CompanyId { get; set; }

    public ReportStatus Status { get; set; }
  }
}
