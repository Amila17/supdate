using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class MetricDataPoint : ModelBase
  {
    /// <summary>
    /// Id of the metric for which forecast values are stored
    /// </summary>
    public int MetricId { get; set; }

    public DateTime Date { get; set; }
    [Editable(false)]
    public string DisplayMonth
    {
      get
      {
        return Date.ToString("MMMM \\'yy", CultureInfo.InvariantCulture);
      }
    }
    public double? Actual { get; set; }
    public double? Target { get; set; }
    [Editable(false)]
    public int? DataSourceId { get; set; }
  }
}
