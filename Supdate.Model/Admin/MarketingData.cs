using System;

namespace Supdate.Model.Admin
{
  public class MarketingData
  {
    public string Email { get; set; }

    public string OldEmail { get; set; }

    public string CompanyName { get; set; }

    public int Areas { get; set; }

    public int Metrics { get; set; }

    public int Goals { get; set; }

    public int ReportsStarted { get; set; }

    public int ReportsCompleted { get; set; }

    public int ReportsSent { get; set; }

    public int ReportsTotal { get; set; }

    public DateTime LastLogin { get; set; }

    public int LoginCount { get; set; }

    public SubscriptionStatus SubscriptionStatus { get; set; }

    public DateTime SubscriptionExpiryDate { get; set; }
  }
}
