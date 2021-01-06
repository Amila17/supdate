var status_complete_string = "<i class='fa fa-check text-complete'></i> Completed ";
var status_inprogress_string = "<i class='fa fa-clock-o text-in-progress'></i> In Progress ";
var newlyComplete = false;
$(function () {



    $('#reportSummaryEntry').on('shown.bs.modal', function () {
        $(document).off('focusin.modal');
    $('#Summary').focus();
  });

  // REPORT STATUS

  updateStatusInUI();

  if (newlyComplete) $('#newlyComplete').modal('show');

  $("#report-toggle-status").click(function () {
    var targetStatus = 1; // assume we're setting it to In Progress
    if ($("#statusId").val() == "1") {
      targetStatus = 2; // actually, we're setting it to Complete
    }

    $("#statusId").val(targetStatus);
    $("#statusManual").val("true");
    updateStatusInUI(true);
    updateStatusAtServer();
  });

  $("#edit-summary-form").submit(function (e) {
      var form = $(this);
      tinyMCE.triggerSave();
    $.ajax({
      url: form.attr('action'),
      type: form.attr('method'),
      data: form.serialize(), // data to be submitted
      success: function (response) {
        $("#reportSummaryEntry").modal("hide");
        displaySuccess("Report summary saved successfully.");
        $("#reportSummary").html(decodeHtml($("#Summary").val()));
        $("#no-summary").hide();
        if (response.newlyCompleted) {
          newlyComplete = true;
          $("#statusId").val(2);
          updateStatusInUI(false);
          $('#newlyComplete').modal('show');
        }
      },
      error: function (response) {
        displayError("An error occurred. Please try again.");
      }
    });
    return false;
  });
});

function updateStatusInUI(isManualChange) {
  if ($("#statusId").val() == "2") {
    $("#report-status-text").html(status_complete_string);
    $("#report-toggle-status").html(status_inprogress_string);
    $("#ftr-btn-send").removeClass("btn-default");
    $("#ftr-btn-view").removeClass("btn-default");
    $("#ftr-btn-send").addClass("btn-success");
    $("#ftr-btn-view").addClass("btn-success");
    $("#ftr-btn-view-lbl").html("View<span class='hidden-xs'> Report</span>");
    if (isManualChange && $("#emailedTo").val() == 0) $('#newlyComplete').modal('show');
  } else {
    $("#report-status-text").html(status_inprogress_string);
    $("#report-toggle-status").html(status_complete_string);
    $("#ftr-btn-send").removeClass("btn-success");
    $("#ftr-btn-view").removeClass("btn-success");
    $("#ftr-btn-send").addClass("btn-default");
    $("#ftr-btn-view").addClass("btn-default");
    $("#ftr-btn-view-lbl").html("Preview");
  }
}

function updateStatusAtServer() {
  console.log($("form#form-status").attr("action"));
  $.ajax({
    type: "POST",
    url: $("form#form-status").attr("action"),
    data: $("form#form-status").serialize(),
    success: function (result) {
      if (result.success) {
      }
    },
    error: function () {
      displayError("An error occurred while saving your report status. Please try again.");
    }
  });
}
