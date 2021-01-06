using System;
using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class MetricViewModel
  {
    public Metric Metric { get; set; }
    public ListHelper ListHelper { get; set; }
  }

  public class MetricSettings
  {
    public Metric Metric { get; set; }

    public IList<Metric> Metrics { get; set; }

    public ListHelper ListHelper { get; set; }
  }
  public class MetricDataViewModel
  {
    public int StartIndex { get; set; }
    public int Year { get; set; }
    public IList<Metric> Metrics { get; set; }

    public IEnumerable<MetricDataPoint> Data { get; set; }
  }
}
