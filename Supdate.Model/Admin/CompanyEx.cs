using System;
using System.Collections.Generic;

namespace Supdate.Model.Admin
{
  public class CompanyEx : Company
  {
    public int OwnerId { get; set; }

    public Guid OwnerUniqueId { get; set; }

    public string OwnerEmail { get; set; }

    public int AreaCount { get; set; }

    public int MetricCount { get; set; }

    public int GoalCount { get; set; }

    public int ReportCount { get; set; }

    public int UserCount { get; set; }

    public UserEx Owner { get; set; }

    public IEnumerable<UserEx> TeamMembers { get; set; }

    public IEnumerable<CompanyTeamMemberInvite> PendingInvites;
  }
}
