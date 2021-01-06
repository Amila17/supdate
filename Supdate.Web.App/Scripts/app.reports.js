$(function () {

  $("#btn-delete-report").click(function (event) {
    var message = "Are you sure you want to delete this report?<br><br>";
    message += "All data you've entered for it will be permanently erased";
    var template = getConfirmNotificationTemplate("document.forms[\"delete-report\"].submit();", "Delete Report?", message);
    displayError(template);
  });

});


function onGoalChanged(element, goalId, goalStatus) {
  if (element.checked) {
    $("#panel_" + goalId).show();
  }
  else {
    $("#panel_" + goalId).hide();
  }
  $("#goalSummary_" + goalId).val("").focus();
  $("#reportGoalInfo_ReportGoalList_" + goalId + "__Status").val(goalStatus);
}

function deleteAttachment(fileId) {
  var message = "Are you sure you want to delete this attachment?";
  var template = getConfirmNotificationTemplate("confirmDeleteAttachment(" + fileId + ")", "Delete Attachment?", message);

  displayError(template);
}

function confirmDeleteAttachment(fileId) {
  var deleteFileError = "There was an error while deleting the attachment. Please try again.";
  $.ajax({
    type: "POST",
    url: miscUrls.deleteAttachment,
    data: { 'fileId': fileId },
    success: function (result) {
      if (result.success) {
        $('html, body').animate({
          scrollTop: $(document).height() - $(window).height()
        },
           600,
           "linear",
           function () {
             setTimeout(function () {
               $('tr[data-attachment-id="' + fileId + '"]').fadeOut(function () {
                 $(this).remove();
               });
             }, 400);

           });

      } else {
        displayError(deleteFileError);
      }
    },
    error: function () {
      displayError(deleteFileError);
    }
  });
}

function validateReportGoalsDetails() {
  var errorMessages = "";

  $("#reportGoalsDetailsForm div[id^='panel_']").each(function () {
    if ($(this).is(':visible')) {
      var reportSummary = $(this).find("textarea[name*='.Summary']").val();
      var reportTitle = $(this).find("input[name*='.Title']").val();

      if (reportSummary === "") {
        errorMessages += "Please enter " + reportTitle + " summary.\n";
      }
    }
  });

  if (errorMessages !== "") {
    displayError(errorMessages);
    return;
  }

  $("form#reportGoalsDetailsForm").submit();
}

function validateReportMetricsDetails() {
  var metricValueRegex = /^-?[0-9]\d*(\.\d+)?$/;
  var errorMessages = "";

  $("#reportMetrics tbody tr").each(function () {
    var metricValue = $(this).find("input[name*='.Actual']").val();
    var metricName = $(this).find("input[name*='.Name']").val();

    if (metricValue !== "" && !metricValueRegex.test(metricValue)) {
      errorMessages += metricName + " value must be a number.\n";
    }
  });

  if (errorMessages !== "") {
    displayError(errorMessages);
    return;
  }

  $("form#reportMetricsForm").submit();
}

function validateReportAreaDetails() {
  tinyMCE.triggerSave();
  var summary = $("#Summary").val();

  if (summary === "") {
    displayError("Please enter report area summary.");
    return;
  }
  $("form#reportAreaForm").submit();
}
