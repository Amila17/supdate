using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace Supdate.Web.App.Models
{
  /// <summary>
  /// UNUSED NOW.
  /// </summary>
  public class FormatHelper
  {
    private const string DefaultUserLanguage = "en";

    public static string DateFormat
    {
      get
      {
        var clientCulture = ClientCulture;

        // Convert the .NET date formats to JavaScript date formats.
        var clientDatepickerFormat = clientCulture.DateTimeFormat.ShortDatePattern.Replace("MM", "mm").Replace("M", "m").Replace("MMMM", "MM").Replace("MMM", "M");
        return clientDatepickerFormat;
      }

    }

    public static void SetClientCulture()
    {
      var clientCulture = ClientCulture;

      // Modify the culture for the current thread.
      Thread.CurrentThread.CurrentCulture = clientCulture;
    }

    private static CultureInfo ClientCulture
    {
      get
      {
        var userLanguage = DefaultUserLanguage;
        var request = HttpContext.Current.Request;

        if (request.UserLanguages != null && request.UserLanguages.Any())
        {
          userLanguage = request.UserLanguages[0];
        }
        var clientCulture = CultureInfo.CreateSpecificCulture(userLanguage);
        return clientCulture;
      }
    }
  }
}
