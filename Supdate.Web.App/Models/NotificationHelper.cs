using System.Web.Mvc;

namespace Supdate.Web.App.Models
{
  public static class NotificationHelper
  {
    public static void SetNotificationMessage(this Controller controller, NotificationType type, string message)
    {
      controller.TempData["messageType"] = (int)type;
      controller.TempData["messageText"] = message;
    }
  }
}
