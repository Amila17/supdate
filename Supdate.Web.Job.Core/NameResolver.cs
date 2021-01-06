using System;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;

namespace Supdate.Web.Job.Core
{
  public class QueueNameResolver : INameResolver
  {
    public string Resolve(string name)
    {
      Console.WriteLine("queue config item is: {0}", name);
      var queueName = CloudConfigurationManager.GetSetting(name);

      if (string.IsNullOrEmpty(queueName))
      {
        Console.WriteLine("Error in the Azure App Settings. Key {0} is missing", name);
      }
      else
      {
        Console.WriteLine("Listening to queue: {0}", queueName);
      }

      return queueName;
    }
  }
}
