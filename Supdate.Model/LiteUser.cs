using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class LiteUser : ModelBase
  {
    public LiteUser()
    {
      OtherCompanies = new List<Company>();
    }

    [Editable(false)]
    public Guid UniqueId { get; set; }

    [DisplayName("Email Address")]
    [Required(ErrorMessage = "Please enter an email address.")]
    [StringLength(200, ErrorMessage = "Email address cannot be longer than 200 characters.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [MaxLength(256)]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string UnConfirmedEmail { get; set; }

    [DisplayName("First Name")]
    [StringLength(200, ErrorMessage = "First Name cannot be longer than 200 characters.")]
    public string FirstName { get; set; }

    [DisplayName("Last Name")]
    [StringLength(200, ErrorMessage = "Last Name cannot be longer than 200 characters.")]
    public string LastName { get; set; }

    public int CompanyId { get; set; }

    public string Company { get; set; }

    public string LogoPath { get; set; }

    [Editable(false)]
    public bool IsCompanyAdmin { get; set; }

    [Editable(false)]
    public SubscriptionStatus SubscriptionStatus { get; set; }

    [Editable(false)]
    public DateTime SubscriptionExpiryDate { get; set; }

    [Editable(false)]
    public bool HasValidSubscription { get; set; }

    [Editable(false)]
    public bool IsAdmin { get; set; }

    [Editable(false)]
    public bool AcceptedLatestTerms { get; set; }

    [DisplayName("Accessible Areas")]
    public int[] AccessibleAreaIds { get; set; }

    [Editable(false)]
    public bool CanManageAreas
    {
      get { return IsCompanyAdmin; }
    }

    [Editable(false)]
    public bool CanWriteReportSummary
    {
      get { return IsCompanyAdmin; }
    }

    [DisplayName("View Reports")]
    [Editable(false)]
    public bool CanViewReports { get; set; }

    [Editable(false)]
    public string DisplayName
    {
      get
      {
        var displayName = string.Format("{0} {1}", FirstName, LastName);

        return (string.IsNullOrWhiteSpace(displayName)) ? Email : displayName;
      }
    }

    public IList<Company> OtherCompanies;
  }
}
