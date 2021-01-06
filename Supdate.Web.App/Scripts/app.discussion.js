$(function() {
  var discussionHub = $.connection.discussionHub;

  if (discussionHub != undefined) {
    discussionHub.client.refreshComments = function(discussionName, commentsHtml, discussionTitle, commentCount) {
      var contentsChanged = refreshComments(discussionName, commentsHtml);
      refreshCommentCount(discussionName, commentCount, contentsChanged);
      refreshDiscussionTitle(discussionName, discussionTitle);
    };

    discussionHub.client.removeComment = function(discussionName, commentId, commentCount) {
      refreshCommentCount(discussionName, commentCount, false);
      $("li[data-comment-id=" + commentId + "]").slideUp(function() {
        $(this).remove();
      });
    };

    $.connection.hub.start().done(function() {
      $(".side-comment").each(function() {
        var discussionName = $(this).data("discussion-name");
        discussionHub.server.subscribe(discussionName);
      });
    });
  }

  $("body").on("click", ".side-comment a.marker", function (e) {
    e.stopPropagation();
    e.preventDefault();
    $($(this).attr('href')).modal('show');
  });

  $("body").on("click", ".comment-wrapper", function (e) {
    e.stopPropagation();
  });

  $("body").on("mouseover", ".commentable-section", function (e) {
    var setCookieVal = "4";
    if (getCookie("explained-comments") != setCookieVal) {
      var duration = 12000;
      var sideComment = $(this).find(".side-comment");
      var marker = $(this).find("a.marker");
      var keepClass = $(sideComment).hasClass("has-comments");

      // keep the marker visible even if user moves mouse out
      $(sideComment).addClass("has-comments");

      // explain what thw marker is for
      flashTip("#" + $(marker).attr("id"), "top", "Whenever you see this icon, you can click it to add a comment.", 0, duration);

      // if necessary, hide marker after tip has gone
      if (!keepClass) {
        setTimeout(function () { $(sideComment).removeClass("has-comments");; }, duration);
      }

      //set a cookie so we don't explain comments again
      setCookie("explained-comments", setCookieVal, 150);
    }
  });

  $(document).on("click", function (e) {
    if ($(e.target).is(".comment-wrapper") === false) {
      $(".side-comment").removeClass("active");
    }
  });

  $("body").on("click", ".commentTrash a", function (e) {
    e.stopPropagation();
    e.preventDefault();
    var el = $(this).closest("li").find(".commentTrashConfirm");
    $(el).height($(this).closest("li").height() -10);
    $(this).hide();
    $(".commentTrashConfirm").fadeOut();
    $(el).fadeIn();
  });

  $("body").on("click", ".commentTrashConfirm a.commentTrashConfirmed", function (e) {
    e.stopPropagation();
    e.preventDefault();
    var discussionUrl = $(this).closest(".commentList").data("discussion-url");
    var commentId = $(this).closest("li").data("comment-id");
    var deleteUrl = discussionUrl + "/comment/" + commentId + "/delete";
    var authorHash = $(this).closest(".side-comment").find("input[name='CommenterHash']").val();
    deleteComment(deleteUrl, authorHash);
  });

  $("body").on("click", ".commentTrashConfirm a.commentTrashCancel", function (e) {
    e.stopPropagation();
    e.preventDefault();
    $(this).closest(".commentTrashConfirm").fadeOut();
    $(this).closest("li").find(".commentTrash a").show();
  });

  $("body").on("shown.bs.modal", ".modal-discussion", function (e) {
    var discussionName = $(this).closest(".side-comment").data("discussion-name");
    autosize($("#comment-" + discussionName)).on('autosize:resized', function () {
      $("#" + discussionName).css("height", "calc(100% - " + (100 + $(this).height()) + "px)");
      scrollCommentsToBottom(discussionName);
    });
    $("#comment-" + discussionName).keypress(function (e) {
      if (e.which == 13) {
        discussionName = $(this).attr("id").substr(8, 1000);
        $("#submit-" + discussionName).click();
        return false;
      }
      return true;
    });
    $("#comment-" + discussionName).focus();
    scrollCommentsToBottom(discussionName);

  });

  $("body").on("hidden.bs.modal", ".modal-discussion", function (e) {
    $(".commentTrashConfirm").hide();
  });

  $(".side-comment").each(function () {
    $(this).find("input[name='Comment.AuthorEmail']").val(
       $(this).find("input[name='CommenterEmail']").val()
      );

    $(this).find("input[name='Comment.AuthorName']").val(
       $(this).find("input[name='CommenterName']").val()
      );

    $(this).find("input[name='Comment.AuthorHash']").val(
       $(this).find("input[name='CommenterHash']").val()
      );
  });

  if (getUrlVars()["discuss"] != undefined) {
    setTimeout(openDiscussionFromQs, 400);
  }

  updateDiscussions();

});

function openDiscussionFromQs() {
  var targetDiscussion = getUrlVars()["discuss"];
  $("#marker-link-" + targetDiscussion).trigger("click");
}

function updateDiscussions() {
  $(".side-comment").each(function (index) {
    var discussionName = $(this).data("discussion-name");

    // update comment counts
    refreshCommentCount(discussionName, null, false);

    // remove unauthorised comment delete buttons
    if ($("#DeleteOwnCommentsOnly").val() == 1) {
      var commentEmail = $(this).find("input[name='CommenterEmail']").val();
      $(this).find("li[data-author-email!='" + commentEmail + "'] .commentTrash").remove();
    }
  });

}

function discussionUpdate_cb(data) {

  var discussionName = data.discussionName;
  $("#comment-" + discussionName).val("");
  $("#comment-" + discussionName).focus();
  autosize.update($("#comment-" + discussionName));

  refreshDiscussionTitle(discussionName, data.title);
  refreshComments(discussionName, data.commentsHtml);
  refreshCommentCount(discussionName, data.commentCount, false);
  updateDiscussions();
}
function refreshComments(discussionName, commentsHtml) {
  var commentList = $("#" + discussionName).find(".commentList");
  var contentsChanged = ($(commentList).html() != commentsHtml)
  $(commentList).html(commentsHtml);
  autoLinkComments();
  scrollCommentsToBottom(discussionName);
  return contentsChanged;
}
function refreshCommentCount(discussionName, commentCount, highlightChange) {
  var commentList = $("#" + discussionName).find(".commentList");
  var sideComment = $(commentList).closest(".side-comment");
  var commentInput = $(sideComment).find("input[name='CommentCount']");
  var oldValue = $(commentInput).val();
  if (commentCount == null) commentCount = oldValue;
  $(commentInput).val(commentCount);

  if (commentCount > 0) {
    $(sideComment).addClass("has-comments");
    $(sideComment).find(".marker span").html(commentCount);
  } else {
    $(sideComment).find(".marker span").html("+");
  }
  if (highlightChange && oldValue != commentCount) {
    if (($("#modal-" + discussionName).data('bs.modal') || {}).isShown) {
       $("#" + discussionName).find(".commentList li").last().highlight();
    } else {
      $(sideComment).find(".marker span").highlight();
    }
  }
}
function refreshDiscussionTitle(discussionName, title) {
  var commentList = $("#" + discussionName).find(".commentList");
  $(commentList).closest(".side-comment").find(".modal-title").text(title);
}
function deleteComment(deleteUrl, authorHash) {
  $.ajax({
    type: "POST",
    url: deleteUrl,
    data: { authorHash: authorHash },
    success: function (result) {
      if (result.success) {
        $("li[data-comment-id=" + result.commentId + "]").closest(".side-comment").find("input[name='CommentCount']").val(result.commentCount);
        $(".commentTrashConfirm").hide();
        $("li[data-comment-id=" + result.commentId + "]").slideUp(function() {
          $(this).remove();
        });
        updateDiscussions();
      }
    },
    error: function () {
      displayError("An error occurred while trying to delete the comment. Please try again.");
    }
  });
}

function autoLinkComments() {
  var autolinker = new Autolinker({ twitter: false, phone: false });
  $(".commentList li").each(function (index) {
    $(this).html(autolinker.link($(this).html()));
  });
}

function scrollCommentsToBottom(discussionName) {
  var commentList = $("#" + discussionName).find(".commentList");
  var height = commentList[0].scrollHeight;
  commentList.scrollTop(height);
}
