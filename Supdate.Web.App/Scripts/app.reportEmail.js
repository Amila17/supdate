var bodyPreviewTimeout;
var bodyChanged;
var senderDetailsChanged;

$(function () {

  // SENDER DETAILS

  $("#senderDetails").on('shown.bs.modal', function () {
    buildCustomSenderString();
   });

  $("#Sender_Preview").click(function () {
    $("#senderDetails").modal("show");
  });
  $("#Sender_Preview").siblings("i").click(function () {
    $("#senderDetails").modal("show");
  });


  $("#UseCustomSender").change(function () {
    toggleCustomSenderInfo();
    senderDetailsChanged = true;
  });

  $("#CustomSenderName").keyup(function () {
    buildCustomSenderString();
    senderDetailsChanged = true;
  });

  $("#CustomSenderEmail").keyup(function () {
    buildCustomSenderString();
    senderDetailsChanged = true;
  });

  $("#saveSenderChanges").click(function () {
    buildCustomSenderString();
    if ($("#UseCustomSender").val() == "true") {
      if ($("#CustomSenderEmail").val() == "") {
        displayError("Enter a valid email address");
        return;
      }
    }

    $("#SendEmail").val("false");
    $.ajax({
      type: "POST",
      url: miscUrls.emailReport,
      data: $("form#reportEmailForm").serialize(),
      success: function (result) {
        if (result.success) {
          $("#senderDetails").modal("hide");
          $("#Sender_Preview").val($("#UseCustomSender option:selected").text());
          if (senderDetailsChanged) {
            $("#Sender_Preview").highlight();
          }
          senderDetailsChanged = false;
        }
      },
      error: function () {
        displayError("An error occurred while saving your sender details. Please try again.");
      }
    });
  });

  // SUBJECT & BODY EDIT
 $("#ReportEmailSubject_Preview").click(function () {
    $("#customizeEmail").modal("show");
 });

 $("#ReportEmailSubject_Preview").siblings("i").click(function () {
   $("#customizeEmail").modal("show");
 });

 $("#ReportEmailSubject").keyup(function () {
    toggleRestoreDefaultsLink();
  });

  $("#ReportEmailBody").keyup(function () {
    toggleRestoreDefaultsLink();
    bodyChanged = true;
  });

  $("#restore-defaults").click(function () {
    document.getElementById("ReportEmailSubject").value = document.getElementById("DefaultReportEmailSubject").value;
    document.getElementById("ReportEmailBody").value = document.getElementById("DefaultReportEmailBody").value;
    toggleRestoreDefaultsLink();
  });

  $("#saveEmailChanges").click(function () {
    $("#SendEmail").val("false");
    $.ajax({
      type: "POST",
      url: miscUrls.emailReport,
      data: $("form#reportEmailForm").serialize(),
      success: function (result) {
        if (result.success) {
          $("#customizeEmail").modal("hide");
          updateSubjectPreview(true);
          updateBodyPreview(true);
        }
      },
      error: function () {
        displayError("An error occurred while saving your email template. Please try again.");
      }
    });
  });

  // OTHER

  $('#reportSendPreviewModal').on('shown.bs.modal', function () {
    $('#PreviewAddress').focus();
  });

  $("button#sendReport").click(function () {
    Ladda.stopAll();
    $("#SendPreview").val("false");
    $("#SendEmail").val("true");

    var receipientsSelected = ($("#recipients tbody tr input[name*='.IsSelected']:checked").length > 0);

    if (receipientsSelected) {
      $.ajax({
        type: "POST",
        url: miscUrls.emailReport,
        data: $("form#reportEmailForm").serialize(),
        success: function (result) {
          if (result.success) {
            if ($("#recipients tbody tr input[name*='.IsSelected']:checked").length > 1) {
              window.location = miscUrls.reportSendThanks;
            } else {
              window.location = miscUrls.showReports;
            }

          }
        },
        error: function () {
          displayError("An error occurred while sending the report. Please try again.");
        }
      });
    } else {
      displayError("Please select at least one recipient to send the report.");
    }
  });

  $("button#sendPreviewEmail").click(function () {
    Ladda.stopAll();
    $("#SendEmail").val("false");
    $("#SendPreview").val("true");
      $.ajax({
        type: "POST",
        url: miscUrls.emailReport,
        data: $("form#reportEmailForm").serialize(),
        success: function (result) {
          if (result.success) {
            $("#reportSendPreviewModal").modal("hide");
            displaySuccess("The preview email has been sent.");
          }
        },
        error: function () {
          displayError("An error occurred while sending the report. Please try again.");
        }
      });
  });


  updateSubjectPreview();
  updateBodyPreview();
  buildCustomSenderString();

});

function updateSubjectPreview(bGlow) {
  var reportDisplayMonth = document.getElementById("ReportDisplayMonth").value;
  var reportDisplayYear = document.getElementById("ReportDisplayYear").value;
  var reportTitle = document.getElementById("ReportTitle").value;
  var userSubject = document.getElementById("ReportEmailSubject").value;

  var previewSubject = userSubject.replace("[TITLE]", reportTitle);
  previewSubject = previewSubject.replace("[MONTH]", reportDisplayMonth);
  previewSubject = previewSubject.replace("[YEAR]", reportDisplayYear);
  if (bGlow && previewSubject != document.getElementById("ReportEmailSubject_Preview").value) {
    $("#ReportEmailSubject_Preview").highlight();
  }
  document.getElementById("ReportEmailSubject_Preview").value = previewSubject;
  toggleRestoreDefaultsLink();
}

function updateBodyPreview(bGlow) {
  if (bGlow && bodyChanged) {
    $("#ReportEmailBody_Preview").highlight();
  }
  document.getElementById("userBody").value = document.getElementById("ReportEmailBody").value;
  $("#bodyPreview").submit();
  bodyChanged = false;
}
function buildCustomSenderString() {
  var customSenderString = "Custom Email Address";
  var name = $("#CustomSenderName").val();
  var email = $("#CustomSenderEmail").val();
  if (email != ""){
    customSenderString = name + " <" + email + ">";
  }
  $("#UseCustomSender option[value='true']").text(customSenderString);
}
function toggleCustomSenderInfo() {
  if ($("#UseCustomSender").val() == "true") {
    $("#sender-info-replyto").slideUp(function () {
      $("#sender-info-custom").slideDown();
    });
    $("#CustomSenderEmail").prop('required', true);
  } else {
    $("#sender-info-custom").slideUp(function() {
      $("#sender-info-replyto").slideDown();
    });
    $("#CustomSenderEmail").prop('required', true);
  }
}
function toggleRestoreDefaultsLink() {
  var bShow = false;
  if (document.getElementById("ReportEmailSubject").value != document.getElementById("DefaultReportEmailSubject").value) {
    bShow = true;
  }
  if (document.getElementById("ReportEmailBody").value != document.getElementById("DefaultReportEmailBody").value) {
    bShow = true;
  }
  if (bShow) {
    $("#restore-defaults").fadeIn();
  } else {
    $("#restore-defaults").fadeOut();
  }
}

