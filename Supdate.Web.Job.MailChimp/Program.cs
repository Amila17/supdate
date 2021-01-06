using Supdate.Web.Job.Core;

namespace Supdate.Web.Job.MailChimp
{
  // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
  public class Program : JobBase
  {
    // Please set the following connection strings in app.config for this WebJob to run:
    // AzureWebJobsDashboard and AzureWebJobsStorage
    static void Main()
    {
      var host = GetJobHost();

      // The following code ensures that the WebJob will be running continuously
      host.RunAndBlock();
    }
  }
}
