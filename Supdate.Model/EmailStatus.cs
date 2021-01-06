using System.ComponentModel.DataAnnotations;

namespace Supdate.Model
{
  public enum EmailStatus
  {
    [Display(Name = "Failed")]
    Failed = 0,

    [Display(Name = "Not sent")]
    New,

    [Display(Name = "Sent")]
    Sent
  }
}