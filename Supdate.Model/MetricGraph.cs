using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Supdate.Model
{
  public class MetricGraph
  {

    public MetricGraph()
    {
      FullGraph = new List<GraphPoint>();
      ThumbnailGraph = new List<GraphPoint>();
    }

    public Guid UniqueId;

    public IList<GraphPoint> FullGraph { get; set; }

    public IList<GraphPoint> ThumbnailGraph { get; set; }

    public string StartDate
    {
      get { return FullGraph.Any(m => m.Actual != 0) ? FullGraph.Where(m => m.Actual != 0).OrderBy(m => m.Date).First().Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); }
    }

    public string EndDate
    {
      get { return FullGraph.Any(m => m.Actual != 0) ? FullGraph.Where(m => m.Actual != 0).OrderByDescending(m => m.Date).First().Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); }
    }

    public string ThumbnailStartDate
    {
      get { return ThumbnailGraph.Any(m => m.Actual != 0) ? ThumbnailGraph.Where(m => m.Actual != 0).OrderBy(m => m.Date).First().Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); }
    }

    public string ThumbnailEndDate
    {
      get { return ThumbnailGraph.Any(m => m.Actual != 0) ? ThumbnailGraph.Where(m => m.Actual != 0).OrderByDescending(m => m.Date).First().Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); }
    }
  }
}
