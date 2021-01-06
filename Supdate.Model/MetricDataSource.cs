using System.ComponentModel.DataAnnotations;

namespace Supdate.Model
{
  public enum MetricDataSource
  {
    [Display(Name = "ChartMogul - Monthly Recurring Revenue")]
    ChartMogulMrr = 1,

    [Display(Name = "ChartMogul - Annualized Run Rate")]
    ChartMogulArr = 2,

    [Display(Name = "ChartMogul - Average Revenue Per Account ")]
    ChartMogulArpa = 3,

    [Display(Name = "ChartMogul - Average Sale Price ")]
    ChartMogulAsp = 4,

    [Display(Name = "ChartMogul - Customer Count ")]
    ChartMogulCustomerCount = 5,

    [Display(Name = "ChartMogul - Customer Churn Rate ")]
    ChartMogulCcr = 6,

    [Display(Name = "ChartMogul - Net MRR Churn Rate ")]
    ChartMogulMrrChurnRate = 7,

    [Display(Name = "ChartMogul - Lifetime Value ")]
    ChartMogulLtv = 8,

    [Display(Name = "Google Analytics - Sessions ")]
    GASessions = 9,

    [Display(Name = "Google Analytics - Users ")]
    GAUsers = 10,

    [Display(Name = "Google Analytics - Page Views ")]
    GAPageViews = 11,

    [Display(Name = "Google Analytics - Unique Page Views ")]
    GAUniquePageViews = 12,

    [Display(Name = "Google Analytics - Page Views / Session ")]
    GAPageViewsPerSessions = 13,

    [Display(Name = "Google Analytics - Goal Completions ")]
    GAGoalCompletions = 14,

    [Display(Name = "Google Analytics - Goal Conversion % ")]
    GAGoalConversion = 15,
  }
}
