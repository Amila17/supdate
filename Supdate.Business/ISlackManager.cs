using System.Collections.Generic;
using Supdate.Business.DataSources;
using Supdate.Model;

namespace Supdate.Business
{
  public interface ISlackManager : IOAuthClientManager
  {
    IList<Webhook> GetWebhooks(int companyId);
  }
}
