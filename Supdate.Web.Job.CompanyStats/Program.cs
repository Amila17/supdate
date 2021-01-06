using System;
using System.Linq;
using SimpleInjector;
using Supdate.Business;
using Supdate.Business.Admin;
using Supdate.DI;

namespace Supdate.Web.Job.CompanyStats
{
  class Program
  {
    private static Container _container;
    private static IAdminManager _adminManager;
    private static ICompanyManager _companyManager;

    public static void Main(string[] args)
    {
      // SimpleInjector container.
      _container = new Container();

      // Register dependencies.
      SimpleInjectorInitializer.InitializeContainer(_container, Lifestyle.Singleton);
      _adminManager = _container.GetInstance<IAdminManager>();
      _companyManager = _container.GetInstance<ICompanyManager>();

      // Update company stats for modified companies.
      UpdateCompanyStats();
    }

    private static void UpdateCompanyStats()
    {
      Console.WriteLine("Checking for modified companies");

      var modifiedCompanies = _adminManager.GetModifiedCompanyList().ToList();
      Console.WriteLine("Found: {0}", modifiedCompanies.Count());

      foreach (var modifiedCompanyId in modifiedCompanies)
      {
        Console.WriteLine("Updating statistics for company id: {0}", modifiedCompanyId);

        // Update statistics.
        _companyManager.UpdateStats(modifiedCompanyId);

        // Get the owner for the company and push the details to mail chimp.
        var user = _companyManager.GetOwner(modifiedCompanyId);
        if (user != null && user.Id > 0)
        {
          Console.WriteLine("Pushing data to MailChimp: {0}", modifiedCompanyId);
          _adminManager.PushToMailChimp(user.Id);
        }

        Console.WriteLine("Done.");
      }
    }
  }
}
