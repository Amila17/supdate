using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Supdate.Data;
using Supdate.Model;
using Supdate.Util;

namespace Supdate.Business
{
  public class EmailService : IIdentityMessageService
  {
    private readonly IGenericEmailManager _genericEmailManager;

    public EmailService()
    {
      _genericEmailManager = new GenericEmailManager(new TemplateManager(), new AzureCloudStorage());
    }

    public async Task SendAsync(IdentityMessage message)
    {
      var mailMessage = new GenericEmail();

      mailMessage.Sender = new GenericEmailAddress(ConfigUtil.NoreplyEmailAddress, ConfigUtil.NoreplyDisplayName);
      mailMessage.ToAddresses.Add(new GenericEmailAddress(message.Destination, string.Empty));
      mailMessage.HtmlBody = message.Body;
      mailMessage.Subject = message.Subject;
      mailMessage.Categories = new[] { "account-activation" };

      await Task.Run(() => _genericEmailManager.QueueEmail(mailMessage));
    }
  }
}
