using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class Webhook : ModelBase
  {
    [ReadOnly(true)]
    public Guid UniqueId { get; set; }
    public int CompanyId { get; set; }

    public string WebhookUrl { get; set; }

    public string ServiceName { get; set; }
    public string ConfigUrl { get; set; }
    public string ConfigInfo1 { get; set; }
    public string ConfigInfo2 { get; set; }

    [DisplayName("Someone updated a Reporting Area")]
    public bool EventReportingAreaUpdated { get; set; }

    [DisplayName("A Recipient viewed your report")]
    public bool EventReportViewed { get; set; }

    [DisplayName("A comment was added to a report")]
    public bool EventReportComment { get; set; }
  }
}
