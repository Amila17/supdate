using Microsoft.Azure.WebJobs.Host;
using SimpleInjector;

namespace Supdate.Web.Job.Core
{
  public class JobActivator : IJobActivator
  {
    private readonly Container _container;

    public JobActivator(Container container)
    {
      _container = container;
    }

    public T CreateInstance<T>()
    {
      return (T) _container.GetInstance(typeof (T));
    }
  }
}
