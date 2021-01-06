namespace Supdate.Model.Admin
{
  public class RegistrationStatistics
  {
    public int Total { get; set; }

    public int TotalEmailConfirmed { get; set; }

    public int TotalReports { get; set; }

    public int TotalReportsEmailed { get; set; }

    public int WindowInDays { get; set; }

    public int TotalInWindow { get; set; }

    public int TotalEmailConfirmedInWindow { get; set; }

    public int TotalReportsInWindow { get; set; }

    public int TotalReportsEmailedInWindow { get; set; }

    public int UnconfirmedAccounts { get; set; }

    public int LoginsInWindow { get; set; }
  }
}
