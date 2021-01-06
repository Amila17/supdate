using Newtonsoft.Json;

namespace Supdate.Model
{
  public class WebhookPayloadAttachment
  {
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }
  }
}