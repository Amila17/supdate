using System;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public interface IDiscussionRepository : ICrudRepository<Discussion>
  {
    Discussion Get(Guid reportGuid, DiscussionTargetType targetType, Guid targetGuid);
    void DeleteComment(Comment model);
  }
}
