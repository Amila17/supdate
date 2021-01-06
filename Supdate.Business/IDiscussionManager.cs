using System;
using Supdate.Model;

namespace Supdate.Business
{
  public interface IDiscussionManager : IManager<Discussion>
  {
    Discussion Get(Guid reportGuid, DiscussionTargetType targetType, Guid targetGuid);
    Discussion AddComment(Discussion discussion, Comment comment);
    void DeleteComment(Comment comment);
  }
}
