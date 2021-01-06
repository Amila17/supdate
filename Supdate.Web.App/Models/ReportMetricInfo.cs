using System;
using System.Collections;
using System.Collections.Generic;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class ReportMetricInfo
  {
    public DateTime ReportDate { get; set; }

    public IList<MetricView> MetricViews { get; set; }

    public ListHelper ListHelper;

    public IList<MetricDataPoint> ToMetricDataPoints()
    {
      IList<MetricDataPoint> mdps = new List<MetricDataPoint>();
      foreach (var metricView in MetricViews)
      {
        mdps.Add(metricView.ToMetricDataPoint());
      }
      return mdps;
    }
  }
}
