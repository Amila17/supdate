using Microsoft.Azure.WebJobs;
using SimpleInjector;
using Supdate.DI;

namespace Supdate.Web.Job.Core
{
  public class JobBase
  {
    protected static Container InitializeDIContainer()
    {
      var container = new Container();

      // Register dependencies.
      SimpleInjectorInitializer.InitializeContainer(container, Lifestyle.Singleton);

      // Verify registrations.
      container.Verify();

      return container;
    }

    protected static JobHost GetJobHost()
    {
      // Create the DI container.
      var container = InitializeDIContainer();

      var configuration = new JobHostConfiguration { JobActivator = new JobActivator(container), NameResolver = new QueueNameResolver() };
      var host = new JobHost(configuration);
      return host;
    }
  }
}
