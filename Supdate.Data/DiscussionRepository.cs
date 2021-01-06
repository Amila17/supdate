using System;
using System.Data;
using System.Linq;
using Dapper;
using Supdate.Data.Base;
using Supdate.Model;

namespace Supdate.Data
{
  public class DiscussionRepository : CrudRepository<Discussion>, IDiscussionRepository
  {
    public Discussion Get(Guid reportGuid, DiscussionTargetType targetType, Guid targetGuid)
    {
      try
      {
        OpenConnection();

        var results = Connection.QueryMultiple("DiscussionGet", new { ReportGuid = reportGuid, targetType = (short)targetType, TargetGuid = targetGuid }, commandType: CommandType.StoredProcedure);

        var discussion = results.Read<Discussion>().FirstOrDefault();

        if (discussion != null)
        {
          discussion.Comments = results.Read<Comment>().ToList();
        }

        return discussion;
      }
      finally
      {
        CloseConnection();
      }
    }

    public void DeleteComment(Comment comment)
    {
      try
      {
        OpenConnection();
        Connection.Delete(comment);
      }
      finally
      {
        CloseConnection();
      }
    }

    public override Discussion Update(Discussion model)
    {
      try
      {
        OpenConnection();

        // see if discussion already exists
        var modelCheck = Get(model.ReportGuId, model.TargetType, model.Target);
        if (modelCheck != null && modelCheck.CompanyId != model.CompanyId)
        {
          // someone being naughty? trying to update a discussion that isn't theirs?
          return model;
        }

        if (modelCheck == null)
        {
          // It doesn't exist yet...

          // - Pull out the comments
          var comments = model.Comments;

          // - Create the discussion
          model = Create(model);

          // - Reattach the comments
          model.Comments = comments;
        }
        else
        {
          base.Update(model);
        }

        if (model.Comments != null)
        {
          foreach (var comment in model.Comments)
          {
            if (!String.IsNullOrWhiteSpace(comment.Text))
            {
              comment.DiscussionId = model.Id;
              if (comment.Id != 0)
              {
                Connection.Update(comment);
              }
              else if (comment.Id == 0)
              {
                var commentId = Connection.Insert<int>(comment);
                var newcomment = Connection.Get<Comment>(commentId);
                comment.CreatedDate = newcomment.CreatedDate;
                comment.UniqueId = newcomment.UniqueId;
              }
            }
          }
        }
        return model;
      }
      finally
      {
        CloseConnection();
      }
    }
  }
}
