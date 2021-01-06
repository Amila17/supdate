using System;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class ReportEmail : ModelBase
  {
    public ReportEmail()
    {
      UniqueId = Guid.NewGuid();
      ViewKey = Guid.NewGuid();
    }

    public Guid UniqueId { get; set; }

    public Guid ViewKey { get; set; }

    public int CompanyId { get; set; }

    [Editable(false)]
    public Guid CompanyGuid { get; set; }

    public int ReportId { get; set; }

    [Editable(false)]
    public Guid ReportGuid { get; set; }

    public int RecipientId { get; set; }

    public EmailStatus Status { get; set; }

    public int Views { get; set; }

    public DateTime? LastViewedDate { get; set; }

    public Recipient Recipient { get; set; }
  }
}
