using System.Collections.Generic;
using System.Web;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class CompanySettings
  {
    public Company Company { get; set; }

    public ListHelper ListHelper { get; set; }

    public HttpPostedFileBase File;

    public IList<Webhook> SlackWebhooks { get; set; }
  }
}
