using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Supdate.Model
{
  public class ReportEmailBuilder
  {
    public DateTime ReportDate { get; set; }

    public int ReportId { get; set; }

    public string ReportTitle { get; set; }

    public string ReportDisplayMonth { get; set; }

    public string ReportDisplayYear { get; set; }

    public string ReportEmailSubject { get; set; }

    public string ReportEmailBody { get; set; }

    public string DefaultReportEmailSubject { get; set; }

    public string DefaultReportEmailBody { get; set; }

    public bool UseCustomSender { get; set; }

    public string CustomSenderName { get; set; }

    public string DefaultSender { get; set; }

    public string SenderPreview { get; set; }

    public string OwnerEmail { get; set; }

    public string PreviewFirstName { get; set; }

    public string PreviewLastName { get; set; }

    public string PreviewAddress { get; set; }

    public bool SendEmail { get; set; }

    public Guid? ReportEmailGuid { get; set; }

    public Guid? ReportEmailViewKey { get; set; }

    public bool SendPreview { get; set; }

    public bool IsSubscriptionActive { get; set; }

    public bool EnableCommenting { get; set; }

    [StringLength(200, ErrorMessage = "Email address cannot be longer than 200 characters.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string CustomSenderEmail { get; set; }

    public ReportStatus ReportStatus;

    public IList<Recipient> RecipientList { get; set; }
  }
}
