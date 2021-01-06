using System.Net.Mail;
using Supdate.Model;

namespace Supdate.Business
{
  public interface IGenericEmailManager
  {
    void SendFromTemplate(string toEmail, string subject, TextTemplate template, TextReplacements replacements);
    void SendBasic(string toName, string toAddress, string subject, string body);
    GenericEmail CreateEmailMessage(string toName, string toAddress, string subject, string body, params string[] categories);
    void QueueEmail(GenericEmail mailMessage);
  }
}
