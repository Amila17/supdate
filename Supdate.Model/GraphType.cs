using System.ComponentModel.DataAnnotations;

namespace Supdate.Model
{
  public enum GraphType
  {
    [Display(Name = "No Graph")]
    NoGraph = 0,

    [Display(Name = "Line Chart")]
    LineChart,

    [Display(Name = "Bar Graph")]
    BarGraph
  }
}
