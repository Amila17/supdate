using System;
using System.ComponentModel.DataAnnotations;

namespace Supdate.Model
{
  public class ReportEmailDetails : ReportEmail
  {
    [Editable(false)]
    public string CompanyName { get; set; }

    [Editable(false)]
    public string CompanyEmail { get; set; }

    [Editable(false)]
    public string ReportTitle { get; set; }

    [Editable(false)]
    public DateTime ReportDate { get; set; }

    [Editable(false)]
    public string ReportSummary { get; set; }

    [Editable(false)]
    public string RecipientName { get; set; }

    [Editable(false)]
    public string RecipientEmail { get; set; }

    [Editable(false)]
    public Guid ReportUniqueId { get; set; }
  }
}
