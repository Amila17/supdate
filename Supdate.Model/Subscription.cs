using System;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class Subscription : ModelBase
  {
    public Subscription()
    {
      Status = SubscriptionStatus.Trialing;
      ExpiryDate = DateTime.UtcNow;
    }

    public int CompanyId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string StripeCustomerId { get; set; }
    public string StripeSubscriptionId { get; set; }

    public void AddDays(int days)
    {
      ExpiryDate = ExpiryDate.AddDays(days);
    }

    public void AddMonths(int months)
    {
      ExpiryDate = ExpiryDate.AddMonths(months);
    }

    public bool IsActive()
    {
      return ExpiryDate >= DateTime.UtcNow;
    }

    public int DaysLeft()
    {
      var timeSpan = ExpiryDate - DateTime.UtcNow;

      return timeSpan.Days;
    }
  }

}
