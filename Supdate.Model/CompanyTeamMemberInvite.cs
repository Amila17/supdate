using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class CompanyTeamMemberInvite : ModelBase
  {
    public int CompanyId { get; set; }

    public string CompanyName { get; set; }

    public string EmailAddress { get; set; }

    public DateTime? UsedDate { get; set; }

    public int? ResultantUserId { get; set; }

    [DisplayName("View Reports")]
    public bool CanViewReports { get; set; }

    [DisplayName("Accessible Areas")]
    public int[] AccessibleAreaIds { get; set; }

    [Editable(false)]
    public Guid UniqueId { get; set; }
  }
}
