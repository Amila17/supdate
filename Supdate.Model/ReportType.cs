using System.ComponentModel.DataAnnotations;

namespace Supdate.Model
{
  public enum ReportType : short
  {
    [Display(Name = "Not Set")]
    NotSet = 0,

    [Display(Name = "Weekly")]
    Weekly = 1,

    [Display(Name = "Monthly")]
    Monthly = 2,

    [Display(Name = "Quarterly")]
    Quarterly = 3
  }
}
