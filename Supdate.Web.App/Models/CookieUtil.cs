using System;
using System.Globalization;
using System.Web;
using Microsoft.Owin;

namespace Supdate.Web.App.Models
{
  public class CookieUtil
  {
    private const string AppProfileKey = "ProfileApp";
    private const string AdminRecordsPerPageKey = "AdminRecordsPerPage";

    public static string AppProfiler
    {
      get
      {
        if (HttpContext.Current.Request.Cookies[AppProfileKey] != null)
        {
          return HttpContext.Current.Request.Cookies[AppProfileKey].Value;
        }

        return string.Empty;
      }

      set { HttpContext.Current.Response.Cookies.Add(new HttpCookie(AppProfileKey, value)); }
    }

    public static int AdminRecordsPerPage
    {
      get
      {
        if (HttpContext.Current.Request.Cookies[AdminRecordsPerPageKey] != null)
        {
          int cookieRecords;
          if (int.TryParse(HttpContext.Current.Request.Cookies[AdminRecordsPerPageKey].Value, out cookieRecords))
          {
            return cookieRecords;
          }
        }

        return 0;
      }

      set { HttpContext.Current.Response.Cookies.Add(new HttpCookie(AdminRecordsPerPageKey, value.ToString(CultureInfo.InvariantCulture))); }
    }

    public static void RemoveAppProfiler()
    {
      var response = HttpContext.Current.Response;

      response.Cookies.Remove(AppProfileKey);
    }

    public static void RemoveAll()
    {
      var response = HttpContext.Current.GetOwinContext().Response;

      var options = new CookieOptions { Expires = DateTime.Now.AddDays(-1) };

      // Specify which cookies to remove.
    }
  }
}
