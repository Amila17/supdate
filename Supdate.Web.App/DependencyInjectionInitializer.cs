using System.Reflection;
using System.Web.Mvc;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using Supdate.DI;
using Supdate.Web.App;
using WebActivator;

[assembly: PostApplicationStartMethod(typeof(DependencyInjectionInitializer), "Initialize")]
namespace Supdate.Web.App
{
  public static class DependencyInjectionInitializer
  {
    /// <summary>Initialize the container and register it as MVC3 Dependency Resolver.</summary>
    public static void Initialize()
    {
      // Did you know the container can diagnose your configuration?
      // Go to: https://simpleinjector.org/diagnostics
      var container = new Container();

      container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

      // Register dependencies.
      SimpleInjectorInitializer.InitializeContainer(container, Lifestyle.Scoped);
      container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

      // Verify registrations.
      container.Verify();

      DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
    }
  }
}
