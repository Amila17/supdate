using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Supdate.Model
{
  public class GraphPoint
  {
    [Editable(false)]
    public Guid UniqueId { get; set; }

    [Editable(false)]
    public DateTime Date { get; set; }
    [Editable(false)]
    public string DateIso
    {
      get
      {
        return Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
      }
    }
    public string Prefix { get; set; }

    public string Suffix { get; set; }

    public bool ThousandsSeparator { get; set; }

    [Editable(false)]
    public string DisplayName
    {
      get
      {
        return Date.ToString("MMM \\'yy");
      }
    }

    public string DisplayNameLong
    {
      get
      {
        return Date.ToString("MMMM \\'yy");
      }
    }

    [Editable(false)]
    public double? Actual { get; set; }

    [Editable(false)]
    public double? Target { get; set; }

    /// <summary>
    /// Set to true if the data point is for the Current Report date
    /// </summary>
    public Boolean IsCurrentDate { get; set; }

    /// <summary>
    /// Set to true if the data point is dated later than the requested report date
    /// </summary>
    public Boolean IsFutureData { get; set; }
  }
}
