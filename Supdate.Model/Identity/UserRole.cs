using Supdate.Model.Base;

namespace Supdate.Model.Identity
{
  public class UserRole : ModelBase
  {
    public int UserId { get; set; }

    public int RoleId { get; set; }
  }
}
