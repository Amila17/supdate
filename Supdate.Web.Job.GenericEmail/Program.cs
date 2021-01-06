using Supdate.Web.Job.Core;

namespace Supdate.Web.Job.GenericEmail
{
  // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
  public class Program : JobBase
  {
    public static void Main()
    {
      var host = GetJobHost();

      // The following code ensures that the WebJob will be running continuously
      host.RunAndBlock();
    }
  }
}
