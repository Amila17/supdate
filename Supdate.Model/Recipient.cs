using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class Recipient : ModelBase
  {
    [ReadOnly(true)]
    public Guid UniqueId { get; set; }

    [StringLength(200, ErrorMessage = "Name cannot be longer than 200 characters.")]
    [DisplayName("First Name")]
    public string FirstName { get; set; }

    [StringLength(200, ErrorMessage = "Name cannot be longer than 200 characters.")]
    [DisplayName("Last Name")]
    public string LastName { get; set; }

    [StringLength(200, ErrorMessage = "Name cannot be longer than 200 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Please enter an email address.")]
    [StringLength(200, ErrorMessage = "Email address cannot be longer than 200 characters.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    public int CompanyId { get; set; }

    [Editable(false)]
    public int ReportEmailId { get; set; }

    [Editable(false)]
    public bool IsSelected { get; set; }

    [Editable(false)]
    public EmailStatus Status { get; set; }

    [Editable(false)]
    public DateTime ReportEmailSentDate { get; set; }

    [Editable(false)]
    public string DisplayName
    {
      get
      {
        var displayName = string.Format("{0} {1}", FirstName, LastName);

        return (string.IsNullOrWhiteSpace(displayName)) ? Email : displayName;
      }
    }
  }
}
