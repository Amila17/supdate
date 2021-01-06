using System.Collections.Generic;
using Newtonsoft.Json;

namespace Supdate.Model
{
  public class WebhookPayload
  {
    public WebhookPayload()
    {
      Attachments = new List<WebhookPayloadAttachment>();
    }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("attachments")]
    public IList<WebhookPayloadAttachment> Attachments { get; set; }
  }
}
