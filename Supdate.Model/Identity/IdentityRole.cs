using Microsoft.AspNet.Identity;
using Supdate.Model.Base;

namespace Supdate.Model.Identity
{
  public class IdentityRole : ModelBase, IRole<int>
  {
    public string Name { get; set; }

    public string Description { get; set; }
  }
}
