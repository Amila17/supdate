using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Supdate.Model.Base;

namespace Supdate.Model
{

  public class MetricDataImport
  {
    public MetricDataImport()
    {
      results = new List<MetricDataPoint>();
      errors = new List<JsonError>();
    }
    public bool success { get; set; }
    public List<MetricDataPoint> results { get; set; }
    public List<JsonError> errors { get; set; }
  }
  public class JsonError
  {
    public string ErrorMessage { get; set; }
  }
}
