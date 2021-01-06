using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using SendGrid;
using Supdate.Util;

namespace Supdate.Web.Job.GenericEmail
{
  public class QueueListener
  {
    #region Generic Email Queue

    public void ProcessGenericEmailQueueMessage([QueueTrigger("%GenericEmailQueue%")] string message, TextWriter log)
    {
      log.WriteLine("Message received: {0}", message);
      Console.WriteLine(message);

      Console.WriteLine("Database key: {0}", ConfigUtil.ConnectionStringName);

      // Send email to the recipient mentioned in the message.
      var email = JsonConvert.DeserializeObject<Model.GenericEmail>(message);
      SendGenericEmail(email, log);
    }

    private void SendGenericEmail(Model.GenericEmail email, TextWriter log)
    {
      var mailMessage = GenericEmailToMailMessage(email);

      log.WriteLine("Sending email to {0} from {1}", mailMessage.To[0].Address, mailMessage.From.Address);
      Console.WriteLine(@"Sending email to {0} from {1}", mailMessage.To[0].Address, mailMessage.From.Address);

      // Send the email.
      var w = new SendGrid.Web(ConfigUtil.SendGridApiKey);
      w.DeliverAsync(mailMessage).Wait();

      log.WriteLine("Sent successfully.");
      Console.WriteLine("Sent successfully.");
    }

    private static ISendGrid GenericEmailToMailMessage(Model.GenericEmail genericEmail)
    {
      // Sender Body and Subject
      var message = new SendGridMessage
                    {
                      From = new MailAddress(genericEmail.Sender.Address, genericEmail.Sender.Name),
                      Subject = genericEmail.Subject,
                      Html = genericEmail.HtmlBody
                    };

      // To, CC and Bcc
      message.To = genericEmail.ToAddresses.Select(m => new MailAddress(m.Address, m.Name)).ToArray();
      message.Cc = genericEmail.CcAddresses.Select(m => new MailAddress(m.Address, m.Name)).ToArray();
      message.Bcc = genericEmail.BccAddresses.Select(m => new MailAddress(m.Address, m.Name)).ToArray();

      // Reply to list
      message.ReplyTo = genericEmail.ReplyToList.Select(m => new MailAddress(m.Address, m.Name)).ToArray();

      // Apply categories.
      message.SetCategories(genericEmail.Categories);

      return message;
    }

    #endregion
  }
}
