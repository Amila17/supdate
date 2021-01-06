using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Supdate.Model.Base;

namespace Supdate.Model
{
  public class Metric : ModelBase
  {
    public Metric()
    {
      GraphType = GraphType.LineChart;
      ThousandsSeparator = true;
      MetricDataPointList = new List<MetricDataPoint>();
    }

    [ReadOnly(true)]
    public Guid UniqueId { get; set; }

    public int CompanyId { get; set; }

    public int? AreaId { get; set; }

    [Required]
    [StringLength(200, ErrorMessage = "{0} cannot be longer than {1} characters.")]
    public string Name { get; set; }

    public string Prefix { get; set; }

    public string Suffix { get; set; }

    public bool ThousandsSeparator { get; set; }

    public bool LowerIsBetter { get; set; }

    public GraphType GraphType { get; set; }

    public short DisplayOrder { get; set; }

    [ReadOnly(true)]
    public int DataPoints { get; private set; }

    [ReadOnly(true)]
    public double? LatestValue { get; private set; }

    [Editable(false)]
    public string LatestValueFormatted
    {
      get { return GetFormattedValue(LatestValue); }
    }

    public int? DataSourceId { get; set; }

    public IList<MetricDataPoint> MetricDataPointList { get; set; }

    public string GetFormattedValue(double? val)
    {
      return (val.HasValue) ? string.Format("{0}{1}{2}", Prefix, string.Format(ThousandsSeparator ? "{0:#,#.######}" : "{0}", val), Suffix) : string.Empty;
    }
  }
}
