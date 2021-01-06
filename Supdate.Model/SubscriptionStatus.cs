using System.ComponentModel.DataAnnotations;

namespace Supdate.Model
{
  public enum SubscriptionStatus
  {
    [Display(Name = "Trialling")]
    Trialing = 0,

    [Display(Name = "Active")]
    Active = 1,

    [Display(Name = "Cancelled")]
    Cancelled = 3,

    [Display(Name = "Unknown")]
    Unknown = 9,
  }
}
