using System;
using System.Collections.Generic;
using System.Data;
using Supdate.Model;

namespace Supdate.Web.App.Models
{
  public class RecipientImportRowViewModel
  {
    public int Index { get; set; }
    public DataTable DataTable { get; set; }
    public DataRow Row { get; set; }
  }
}
