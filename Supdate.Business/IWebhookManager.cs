using Supdate.Model;

namespace Supdate.Business
{
  public interface IWebhookManager : IManager<Webhook>
  {
    void ReportingAreaUpdated(int companyId, LiteUser currentUser, ReportArea reportArea, string reportUrl);

    bool ReportViewed(int companyId, ReportEmail reportEmail, Report report);

    bool CommentPosted(int companyId, Discussion discussion, Comment comment);
  }
}
