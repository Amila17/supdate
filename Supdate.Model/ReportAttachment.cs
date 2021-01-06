using System;
using System.ComponentModel;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class ReportAttachment : ModelBase
  {
    [ReadOnly(true)]
    public Guid UniqueId { get; set; }

    public int CompanyId { get; set; }

    public int ReportId { get; set; }

    public int? AreaId { get; set; }

    public string MimeType { get; set; }

    public string FileName { get; set; }

    public string FilePath { get; set; }

    public string Description { get; set; }
  }
}
