deleteAreaPromptMsg = "Are you sure you want to delete this Reporting Area?<br><br>";
deleteAreaPromptMsg += "Any Goals and Metrics assigned to it will be kept but unassigned.<br><br>";
deleteAreaPromptMsg += "Updates on this Reporting Area will be PERMANENTLY DELETED.";

deleteGoalPromptMsg = "Are you sure you want to delete this Goal?<br><br>";
deleteGoalPromptMsg += "Any updates ever provided for it will be PERMANENTLY DELETED.";

$(function() {

  if (getUrlVars()["showSlack"] === "1") {
    $("#slack-table tbody tr:eq(0) td:first-child").click();
  }

});

function deleteExternalApiAuth_cb(data, entityId) {
  $(".modal").modal("hide");
  var div = $(".panel[data-auth-id=" + entityId + "]");
  $(div).removeClass("configured");
  setTimeout(function () {
    displaySuccess("External API Credentials have been removed.");
  }, 600);
  toggleMetricsGuideNotification();

}
function changeemail_cb(data) {
  $(".modal").modal("hide");
  setTimeout(function () {
    displaySuccess("We have sent you an email to confirm your new email address.");
  }, 600);
}
function SaveTeamMember_cb(data, modalId) {
  $("#" + modalId).modal("hide");
  areaListEl = $("#" + data.teamMember.UniqueId + "_AreaList");
  var originalAreaList = $.trim($(areaListEl).html());
  $(areaListEl).html(data.areaListString);
  if (data.areaListString != originalAreaList) $(areaListEl).highlight();
  setTimeout(function() {
    displaySuccess("Team Member successfully updated.");
  }, 600);
}
function attachment_cb(data) {
  $("#attachments-table tbody").append(data.html);
  $("#attachments-table").show();
  $('#attachment-form').trigger("reset");
  $("#file-description-container").slideUp();
  setFaAutoMimeTypes();
  displaySuccess("File successfully uploaded.");
}
function webhook_cb(data) {
  $(".modal").modal("hide");
  setTimeout(function () {
    displaySuccess("Settings successfully updated.");
  }, 600);
}
function googleAnalyticsSetSiteId_cb(data) {
  $(".modal").modal("hide");
  setTimeout(function () {
    displaySuccess("Google Analytics site settings successfully updated.");
  }, 600);
  toggleMetricsGuideNotification();
}
function toggleMetricsGuideNotification() {
  if ($("#MappedMetricsCount").val() === "0" && $(".btn-editconfig").is(":visible")) {
    $("#map-metrics-guide").fadeIn();
  } else {
    $("#map-metrics-guide").fadeOut();
  }
}
function addNewForecast() {
  var startDate = "";

  if ($("#metricForecasts div").length > 0) {
    startDate = $("#metricForecasts div:first input[id^='metricForecastMonth']").val();
  }

  var startIndex = parseInt($("#NextForecastStartIndex").val());

  $("#NextForecastStartIndex").val(startIndex);
  $("#currentIndex").val(startIndex);
  $("#lastDate").val(startDate);

  var form = $("form#newForecastRows");
  $.ajax({
    type: "POST",
    url: $(form).attr("action"),
    data: $(form).serialize(),
    success: function (msg) {
      var newIndex = parseInt($("#NextForecastStartIndex").val()) + parseInt($("#numberOfNewEntries").val());
      $("#NextForecastStartIndex").val(newIndex);
      $("#metricForecasts").prepend(msg.html);
      $("#metricForecasts div:first input[name$='.Value']").first().focus();

      $("#metricForecasts").show();
      $("#metricForecasts_fade").show();

    },
    error: function () {
      displayError("An error occurred while adding forecast rows. Please try again.");
    }
  });
 }


function promptDeleteEntity(entityTypeName, entityId, promptTitle, promptMsg) {
  toastr.remove();
  if (promptTitle == null) promptTitle = "Delete " + entityNiceNames[entityTypeName] + "?";
  if (promptMsg == null) promptMsg = "Are you sure you want to delete this " + entityNiceNames[entityTypeName];
  var template = getConfirmNotificationTemplate("deleteEntity(\"" + entityTypeName + "\",\"" + entityId + "\")", promptTitle, promptMsg);
  displayError(template);
  centerToast();
}
function centerToast() {
  var $notifyContainer = $('#toast-container').closest('.toast-top-center');
  if ($notifyContainer) {
    // align center
    var notifyHeight = $($notifyContainer).height();
    var windowHeight = $(window).height() - notifyHeight;
    $notifyContainer.css("margin-top", windowHeight / 2);
  }
}
function deleteEntity(entityTypeName, entityId) {
  var deleteEntityError = "There was an error while deleting the " + entityNiceNames[entityTypeName] + ". Please try again.";
  var deleteEntitySuccess = "The " + entityNiceNames[entityTypeName] + " has been deleted.";
  var postData = '{"' + entityTypeName + 'Id": "' + entityId + '"}';
  $.ajax({
    type: "POST",
    url: deleteUrls[entityTypeName],
    data: JSON.parse(postData),
    success: function (result) {
      if (result.success) {
        // look for delete button
        if ($("*[data-delete-entity-id=" + entityId + "]").data("delete-entity-cb")) {
          var cbFunction = $("*[data-delete-entity-id=" + entityId + "]").data("delete-entity-cb")
          window[cbFunction].call(null, result, entityId);
        }
        else
        {
          toastr.remove();
          $(".modal").modal("hide");
          $("tr[data-" + entityTypeName + "-id='" + entityId + "']").fadeOut(800, function () {
            $("tr[data-" + entityTypeName + "-id='" + entityId + "']").remove();
            displaySuccess(deleteEntitySuccess);
          });
        }
      } else {
        displayError(deleteEntityError);
      }
    },
    error: function () {
      displayError(deleteEntityError);
    }
  });
}


