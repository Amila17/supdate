using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class Company : ModelBase
  {
    [Required(ErrorMessage = "Please enter a Company name.")]
    [StringLength(300, ErrorMessage = "Company name cannot be longer than 300 characters.")]
    public string Name { get; set; }

    public string TwitterHandle { get; set; }

    public DateTime? StartMonth
    {
      get
      {
        return !string.IsNullOrEmpty(StartMonthText) ? DateTime.ParseExact(StartMonthText, "MMMM yyyy", CultureInfo.InvariantCulture) : default(DateTime?);
      }

      set
      {
        if (value.HasValue)
        {
          StartMonthText = value.Value.Year > 2000 ? value.Value.ToString("MMMM yyyy", CultureInfo.InvariantCulture) : string.Empty;
        }
        else
        {
          StartMonthText = string.Empty;
        }
      }
    }

    [Editable(false)]
    public string StartMonthText { get; set; }

    public string LogoPath { get; set; }

    [DisplayName("Enable Commenting on Reports")]
    public bool EnableCommenting { get; set; }
    public ReportType ReportType { get; set; }
    public string ReportTitle { get; set; }
    public string ReportEmailSubject { get; set; }
    public string ReportEmailBody { get; set; }
    public bool UseCustomSender { get; set; }
    public string CustomSenderName { get; set; }

    [StringLength(200, ErrorMessage = "Email address cannot be longer than 200 characters.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string CustomSenderEmail { get; set; }

    [StringLength(255)]
    [DisplayName("Billing Name")]
    public string BillingName { get; set; }

    [StringLength(255)]
    [DisplayName("Address Line 1")]
    public string BillingAddress1 { get; set; }

    [StringLength(255)]
    [DisplayName("Address Line 2")]
    public string BillingAddress2 { get; set; }

    [StringLength(255)]
    [DisplayName("City")]
    public string BillingCity { get; set; }

    [StringLength(255)]
    [DisplayName("Postcode")]
    public string BillingPostCode { get; set; }

    [StringLength(255)]
    [DisplayName("Country")]
    public string BillingCountry { get; set; }

    [Editable(false)]
    public string BillingAddress
    {
      get
      {
        var sb = new StringBuilder();
        Append(ref sb, BillingName);
        Append(ref sb, BillingAddress1, Environment.NewLine);
        Append(ref sb, BillingAddress2, Environment.NewLine);
        Append(ref sb, BillingCity, Environment.NewLine);
        Append(ref sb, BillingPostCode, Environment.NewLine);
        Append(ref sb, BillingCountry, Environment.NewLine);
        var billingAddress = sb.ToString();

        return string.IsNullOrWhiteSpace(billingAddress) ? Name : billingAddress;
      }
    }
    [ReadOnly(true)]
    public Guid UniqueId { get; set; }

    public IEnumerable<ReportPermalink> Permalinks { get; set; }

    private static void Append(ref StringBuilder sb, string value, string prefixIfAlreadyHasContent = null)
    {
      if (!string.IsNullOrWhiteSpace(value))
      {
        if (sb.Length > 0 && prefixIfAlreadyHasContent != null)
        {
          sb.Append(prefixIfAlreadyHasContent);
        }

        sb.Append(value);
      }
    }
  }
}
