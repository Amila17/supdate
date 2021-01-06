using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class ExternalApiAuth : ModelBase
  {
    [ReadOnly(true)]
    public Guid UniqueId { get; set; }
    public int CompanyId { get; set; }

    [Editable(false)]
    public ExternalApi ExternalApi
    {
      get
      {
        return ExternalApi.ChartMogul.GetById(ExternalApiId);
      }
    }

    public int ExternalApiId { get; set; }
    public string Token { get; set; }
    public string Key { get; set; }
    public string ConfigData { get; set; }
  }
}
