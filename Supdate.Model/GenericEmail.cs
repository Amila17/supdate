using System.Collections.Generic;

namespace Supdate.Model
{
  public class GenericEmail
  {
    public GenericEmail()
    {
      ToAddresses = new List<GenericEmailAddress>();
      CcAddresses = new List<GenericEmailAddress>();
      BccAddresses = new List<GenericEmailAddress>();
      ReplyToList = new List<GenericEmailAddress>();
    }

    public List<GenericEmailAddress> ToAddresses { get; set; }
    public List<GenericEmailAddress> CcAddresses { get; set; }
    public List<GenericEmailAddress> BccAddresses { get; set; }
    public List<GenericEmailAddress> ReplyToList { get; set; }
    public GenericEmailAddress Sender { get; set; }
    public string Subject { get; set; }
    public string TextBody { get; set; }
    public string HtmlBody { get; set; }

    public string[] Categories { get; set; }
  }
}
