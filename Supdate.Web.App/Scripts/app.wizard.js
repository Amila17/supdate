var wizardSteps = [
  "wizard-areas",
  "wizard-metric-names",
  "wizard-team",
  "wizard-recipients",
  "wizard-complete"
];
var wizardTitles = [
  "Reporting Areas",
  "Metrics",
  "Team",
  "Recipients",
  "Complete!"
];
var itemsCreated = 0;
var itemsFailed = 0;
var nextButtonGlowTimeout;

$(function () {


  $(".wizard-tagger.wizard-tagger-email").tagsinput({
    tagClass: "label wizard-label",
    trimValue: true,
    // include 'enter' (13) and space (32) as well as comma (44) to submit email addresses
    confirmKeys: [13, 32, 44]
  });
  $(".wizard-tagger").tagsinput({
    tagClass: "label wizard-label",
    trimValue: true
  });

  $('#team-members-to-create').tagsinput('input').attr("placeholder", "Enter Email Addresses");
  $('#recipients-to-create').tagsinput('input').attr("placeholder", "Enter Email Addresses");;

  // initialize step 0 - areas
  $('#areas-to-create').tagsinput('focus');


  // next button
  $("#wizard-next").click(function () {
    toastr.remove(); //remove any error msgs
    var addData = {};
    switch (Number($("#wizard-step").val())) {
      case 0:
        // CREATE AREAS
        wizardCreateItems("areas-to-create", "Area", createUrls.area, "Name");
        break;
      case 1:
        // CREATE METRICS
        addData["Metric.AreaId"] = -2;
        wizardCreateItems("metrics-to-create", "Metric", createUrls.metric, "Metric.Name", addData);
        break;
      case 2:
        // INVITE TEAM MEMBERS
        // skip if none provided
        if ($("#team-members-to-create").tagsinput("items").length === 0) {
          wizardNextStep();
          break;
        }
        addData["WelcomeMessage"] = $("#invite-message").val();
        addData["accessAllAreas"] = true;
        addData["TeamMember.CanViewReports"] = true;
        wizardCreateItems("team-members-to-create", "Team Member", createUrls.teamMember, "TeamMember.Email", addData);
        break;
      case 3:
        // CREATE RECIPIENTS
        // skip if none provided
        if ($("#recipients-to-create").tagsinput("items").length === 0) {
          wizardNextStep();
          break;
        }
        addData["FirstName"] = "auto-gen-from-email";
        wizardCreateItems("recipients-to-create", "Recipient", createUrls.recipient, "Email", addData);
        break;
      default:
        self.location.href = miscUrls.showReports;
        break;
    }
  });

  // suggestions
  $(".tag-adder").click(function () {
    var target = $(this).data("target");
    $(this).slideUp("slow", function () {
      $("#" + target).tagsinput("add", $(this).text());
      $(this).remove();
      if ($("button.tag-adder[data-target=" + target + "]").length === 0) {
        $("p.help-block[data-target=" + target + "]").slideUp();
      }
      $("#" + target).tagsinput('focus');
    });
  });

  //skip links
  $(".wizard-skip").click(function () {
    wizardNextStep();
  });


  // highlight 'next' button on first screen
  $('#areas-to-create').on('itemAdded', function (event) {
    clearTimeout(nextButtonGlowTimeout);
    nextButtonGlowTimeout = setTimeout(function () { $("#wizard-next").highlight(); }, 4000);
  });

  // validate email addresses
  $(".wizard-tagger-email").on("beforeItemAdd", function (event) {
    toastr.remove();
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    if (!pattern.test(event.item)) {
      displayError("'" + $('<div/>').text(event.item).html() + "' is not a valid email address.");
      $("#" + event.currentTarget.id).tagsinput("input").val(event.item); //doesn't work?
      event.cancel = true;
    } else {
      $("#" + event.currentTarget.id).tagsinput('input').attr("placeholder", "");

    }
  });
});

function getWizardProgress() {
  var totalSteps = wizardSteps.length - 1;
  var stepNumber = $("#wizard-step").val();
  return Math.round((100 / totalSteps) * stepNumber);
}

function updateWizardProgressBar(delay) {
  var progress = getWizardProgress();
  setTimeout(function () {
    $('#wizard-progress-bar').css('width', progress + '%').attr('aria-valuenow', progress);
  }, delay);
}

function wizardNextStep() {
  Ladda.stopAll();
  itemsCreated = 0;
  itemsFailed = 0;
  var stepNumber = $("#wizard-step").val();
  stepNumber++;
  $("#wizard-step").val(stepNumber);
  $("#wizard-title").fadeOut();
  $("#" + wizardSteps[stepNumber - 1]).slideUp(function () {
    $("#" + wizardSteps[stepNumber]).slideDown(function () {
      $("#" + wizardSteps[stepNumber] + " .wizard-tagger").tagsinput('focus');
      $("#wizard-title").text(wizardTitles[stepNumber]);
      $("#wizard-title").fadeIn();
      updateWizardProgressBar(1000);
    });
  });
}

function wizardCreateItems(itemsTagger, itemName, postUrl, postName, postData) {
  var items = $("#" + itemsTagger).tagsinput("items");
  if (items.length === 0) {
    displayError("You must create at least one " + itemName);
  } else {
    for (var i = 0; i < items.length; i++) {
      var item = items[i];
      var jsonObj = {};
      jsonObj[postName] = item;

      // deal with additional data
      if (postData != null) {
        for (var prop in postData) {
          if (postData.hasOwnProperty(prop)) {
            jsonObj[prop] = postData[prop];
          }
        }
      }

      $.ajax({
        type: "POST",
        url: postUrl,
        dataType: 'json',
        accept: "application/json; charset=utf-8",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObj),
        success: function (msg, i) {
          itemsCreated++;
        },
        async: itemName !== "Area",
        error: function () {
          itemsFailed++;
          displayError("An error occurred while creating one of your items.");
        },
        complete: function () {
          if ((itemsFailed + itemsCreated) === items.length) {
            wizardNextStep();
          }
        }
      });
    }
  }
}
