using SimpleInjector;
using Supdate.Business;
using Supdate.Business.Admin;
using Supdate.Business.DataSources;
using Supdate.Data;
using Supdate.Data.Admin;
using Supdate.Data.Base;
using Supdate.Model;
using Supdate.Model.Admin;

namespace Supdate.DI
{
  public class SimpleInjectorInitializer
  {
    public static void InitializeContainer(Container container, Lifestyle lifestyle)
    {
      // Managers
      container.Register<ICompanyManager, CompanyManager>(lifestyle);
      container.Register<IDiscussionManager, DiscussionManager>(lifestyle);
      container.Register<IReportAreaManager, ReportAreaManager>(lifestyle);
      container.Register<IReportGoalManager, ReportGoalManager>(lifestyle);
      container.Register<IReportManager, ReportManager>(lifestyle);
      container.Register<IMetricManager, MetricManager>(lifestyle);
      container.Register<ILogoManager, LogoManager>(lifestyle);
      container.Register<IRecipientManager, RecipientManager>(lifestyle);
      container.Register<IReportEmailManager, ReportEmailManager>(lifestyle);
      container.Register<IReportAttachmentManager, ReportAttachmentManager>(lifestyle);
      container.Register<IAreaManager, AreaManager>(lifestyle);
      container.Register<IGoalManager, GoalManager>(lifestyle);
      container.Register<IGenericEmailManager, GenericEmailManager>(lifestyle);
      container.Register<ITemplateManager, TemplateManager>(lifestyle);
      container.Register<IAdminManager, AdminManager>(lifestyle);
      container.Register<IWebhookManager, WebhookManager>(lifestyle);
      container.Register<ISlackManager, SlackManager>(lifestyle);
      container.Register<ISubscriptionManager, SubscriptionManager>(lifestyle);
      container.Register<IChartMogulApiManager, ChartMogulApiManager>(lifestyle);
      container.Register<IGoogleAnalyticsApiManager, GoogleAnalyticsApiManager>(lifestyle);
      container.Register<IExternalApiAuthManager, ExternalApiAuthManager>(lifestyle);
      container.Register<IGoogleOAuthDataStore, GoogleOAuthDataStore>(lifestyle);
      container.Register<IGoogleAuthorizer, GoogleAuthorizer>(lifestyle);
      container.Register<ITermsAndConditionsManager, TermsAndConditionsManager>(lifestyle);

      // Repositories
      container.Register<ICompanyRepository, CompanyRepository>(lifestyle);
      container.Register<IDiscussionRepository, DiscussionRepository>(lifestyle);
      container.Register<IGoalRepository, GoalRepository>(lifestyle);
      container.Register<IReportAreaRepository, ReportAreaRepository>(lifestyle);
      container.Register<IMetricRepository, MetricRepository>(lifestyle);
      container.Register<ICloudStorage, AzureCloudStorage>(lifestyle);
      container.Register<ICrudRepository<Recipient>, CrudRepository<Recipient>>(lifestyle);
      container.Register<IMetricDataPointRepository, MetricDataPointRepository>(lifestyle);
      container.Register<IReportRepository, ReportRepository>(lifestyle);
      container.Register<IReportGoalRepository, ReportGoalRepository>(lifestyle);
      container.Register<IRecipientRepository, RecipientRepository>(lifestyle);
      container.Register<IReportEmailRepository, ReportEmailRepository>(lifestyle);
      container.Register<ICrudRepository<ReportAttachment>, CrudRepository<ReportAttachment>>(lifestyle);
      container.Register<IAreaRepository, AreaRepository>(lifestyle);
      container.Register<IAdminRepository, AdminRepository>(lifestyle);
      container.Register<ICrudRepository<UserConfirmation>, CrudRepository<UserConfirmation>>(lifestyle);
      container.Register<ICrudRepository<UtmInfo>, CrudRepository<UtmInfo>>(lifestyle);
      container.Register<ICrudRepository<Webhook>, CrudRepository<Webhook>>(lifestyle);
      container.Register<ISubscriptionRepository, SubscriptionRepository>(lifestyle);
      container.Register<IExternalApiAuthRepository, ExternalApiAuthRepository>(lifestyle);
      container.Register<ITermsAndConditionsRepository, TermsAndConditionsRepository>(lifestyle);
    }
  }
}
