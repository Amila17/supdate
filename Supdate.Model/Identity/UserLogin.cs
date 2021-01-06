using Supdate.Model.Base;

namespace Supdate.Model.Identity
{
  public class UserLogin : ModelBase
  {
    public int UserId { get; set; }

    public string LoginProvider { get; set; }

    public string ProviderKey { get; set; }
  }
}
