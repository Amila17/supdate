using Supdate.Model.Base;

namespace Supdate.Model.Identity
{
  public class UserClaim : ModelBase
  {
    public int UserId { get; set; }

    public string ClaimType { get; set; }

    public string ClaimValue { get; set; }
  }
}
