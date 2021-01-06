using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class TeamMemberViewModel
  {
    public LiteUser TeamMember { get; set; }
    public ListHelper ListHelper;
    [DisplayName("Welcome Message")]
    public string WelcomeMessage { get; set; }
  }
  public class TeamViewModel
  {
    public IEnumerable<LiteUser> TeamMembers;
    public IEnumerable<CompanyTeamMemberInvite> Invites;
    public ListHelper ListHelper;
  }
}
