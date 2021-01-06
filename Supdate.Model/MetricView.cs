using System;
using System.ComponentModel.DataAnnotations;

namespace Supdate.Model
{
  public class MetricView : Metric
  {
    public int MetricDataPointId { get; set; }
    public DateTime Date { get; set; }
    public int ReportId { get; set; }
    public Guid ReportUniqueId { get; set; }
    /// <summary>
    /// Stores the value for current
    /// </summary>
    public double? Actual { get; set; }

    [Editable(false)]
    public string ValueFormatted
    {
      get { return GetFormattedValue(Actual); }
    }

    /// <summary>
    /// Forecast value specified for the current month.
    /// This value will be filled only when metrics are being entered.
    /// Note: This is only filled while creating this object, not for db storage
    /// </summary>
    [Editable(false)]
    public double? Target { get; set; }

    [Editable(false)]
    public string TargetFormatted
    {
      get { return GetFormattedValue(Target); }
    }

    [Editable(false)]
    public double? LastMonthActual { get; set; }

    [Editable(false)]
    public string LastMonthActualFormatted
    {
      get { return GetFormattedValue(LastMonthActual); }
    }

    [Editable(false)]
    public double PercentageChangeFromLastMonth
    {
      get
      {
        var result = 0d;

        if (LastMonthActual.HasValue && !LastMonthActual.Value.Equals(0))
        {
          result = ((Actual ?? 0d) - LastMonthActual.Value) * 100 / LastMonthActual.Value;
        }
        else if (Actual.HasValue && !Actual.Value.Equals(0d))
        {
          result = 100;
        }

        return result;
      }
    }

    [Editable(false)]
    public double PercentageChangeFromTarget
    {
      get
      {
        var result = 0d;

        if (Target.HasValue && !Target.Value.Equals(0))
        {
          result = ((Actual ?? 0d) - Target.Value) * 100 / Target.Value;
        }
        else if (Actual.HasValue && !Actual.Value.Equals(0d))
        {
          result = 100;
        }

        return result;
      }
    }

    public MetricDataPoint ToMetricDataPoint()
    {
      return new MetricDataPoint { Id = MetricDataPointId, MetricId = Id, Actual = Actual, Target = Target, Date = Date};
    }
  }
}
