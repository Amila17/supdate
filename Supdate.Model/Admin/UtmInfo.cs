using Supdate.Model.Base;

namespace Supdate.Model.Admin
{
  public class UtmInfo : ModelBase
  {
    public int UserId { get; set; }
    public string Source { get; set; }
    public string Medium { get; set; }
    public string Term { get; set; }
    public string Content { get; set; }
    public string Campaign { get; set; }
  }
}
