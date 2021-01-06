using System.Text;
using System.Web.Mvc;
using GravatarHelper.Extensions;
using Supdate.Util;

namespace Supdate.Web.App.Extensions
{
  public static class HtmlHelperExtensions
  {
    public static MvcHtmlString ReportEditClass(this HtmlHelper htmlHelper, bool canEdit)
    {
      if (!canEdit)
      {
        return MvcHtmlString.Empty;
      }

      return MvcHtmlString.Create("sup-editable");
    }

    public static MvcHtmlString ReportCommentClass(this HtmlHelper htmlHelper, bool canComment)
    {
      if (!canComment)
      {
        return MvcHtmlString.Empty;
      }

      return MvcHtmlString.Create("commentable-section");
    }

    public static MvcHtmlString GravatarWithDefault(this HtmlHelper htmlHelper, string email, int imageSize)
    {
      return htmlHelper.Gravatar(email, imageSize, ConfigUtil.DefaultGravatarImage);
    }

    public static MvcHtmlString ReportEditDataTags(this HtmlHelper htmlHelper, bool canEdit, string dataType, string dataName)
    {
      if (!canEdit)
      {
        return MvcHtmlString.Empty;
      }

      var sb = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(dataType))
      {
        sb.AppendFormat(" data-type='{0}' ", dataType);
      }

      if (!string.IsNullOrWhiteSpace(dataName))
      {
        sb.AppendFormat(" data-name='{0}' ", dataName);
      }

      return MvcHtmlString.Create(sb.ToString());
    }
  }
}
