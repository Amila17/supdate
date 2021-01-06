using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Supdate.Web.App.Hubs
{
  public class DiscussionHub : Hub
  {
    public Task Subscribe(string disucssionName)
    {
      return Groups.Add(Context.ConnectionId, disucssionName);
    }
  }
}
