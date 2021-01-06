var metricDataValueRegex = /^-?[0-9]\d*(\.\d+)?$/;

$(function () {

  if ($("#metricDataContainer").length) {

    $("#metricDataContainer").find(".table-metric-data").first().show();

    if (getUrlVars()["show"] == "targets") {
      toggleTargetsAndActuals(false, false);
    }

    if (getUrlVars()["metric"] === undefined) {
      $(".table-metric-data:visible").find("input[type=text]:visible").first().focus();
    } else {
     // console.log('selecting');
      setTimeout(function() { selectMetricDataRow(getUrlVars()["metric"]); }, 500);
    }

  }

  $("body").on("click", "#metric-data-toggle-targets", function (e) {
    toggleTargetsAndActuals(false, true);
  });

  $("body").on("click", "#metric-data-toggle-actuals", function (e) {
    toggleTargetsAndActuals(true, true);
  });

  $("body").on("keyup", " table.table-metric-data tr td input[type=text]", function (e) {

    if ($(this).hasClass("error")) {
      if ($(this).val() == "" || metricDataValueRegex.test($(this).val())) {
        $(this).removeClass("error");
      }
    }

    var upKey = 38;
    var downKey = 40;
    if (e.which == downKey || e.which == upKey) {
      var td = $(this).closest("td");
      var tr = $(td).closest("tr");
      var tbody = $(tr).closest("tbody");
      var targetTrIndex = tr.index() + 1;
      var targetTdIndex = td.index();
      if (e.which == upKey) targetTrIndex = tr.index() - 1;
      var lastRow = $(tbody).find("tr").length;

      if (targetTrIndex >= 0 && targetTrIndex < lastRow)
      {
        var cell = tbody[0].rows[targetTrIndex].cells[targetTdIndex]; // This is a DOM "TD" element
        var targetCell = $(cell); // Now it's a jQuery object.
        var targetInput = $(targetCell).find("input[type=text]:visible");
        $(targetInput).focus();
        $(targetInput).select();
      }
    }
  });

  $("#save-metric-data").click(function () {
    if (validateMetricDataInputs()) {
      $("#metricsDataForm").submit();
    }

  });

  $("#year-minus").click(function (e) {
    metricData_MoveYears(-1);
    e.preventDefault();
  });

  $("#year-plus").click(function (e) {
    metricData_MoveYears(1);
    e.preventDefault();
  });
});

function validateMetricDataInputs() {
  var errorMessages_Target = "";
  var errorMessages_Actual = "";

  $(".table-metric-data tbody tr td").each(function () {
    if ($(this).find("input[name*='.Actual']").length) {
      var actual = $(this).find("input[name*='.Actual']");
      var target = $(this).find("input[name*='.Target']");
      var metricName = $(this).closest("tr[data-metric-name]").data("metric-name");
      var metricDate = $(this).data("display-date");

      if ($(actual).val() !== "" && !metricDataValueRegex.test($(actual).val())) {
        errorMessages_Actual += " " + metricDate + " - " + metricName + "\n";
        $(actual).addClass("error");
      }
      if ($(target).val() !== "" && !metricDataValueRegex.test($(target).val())) {
        errorMessages_Target += " " + metricDate + " - " + metricName + "\n";
        $(target).addClass("error");
      }
    }
  });

  if (errorMessages_Actual !== "" || errorMessages_Target !== "") {
    var errorMessages = "";
    if (errorMessages_Actual !== "") {
      errorMessages = "INVALID ACTUALS\n" + errorMessages_Actual;
    }
    if (errorMessages_Target !== "") {
      if (errorMessages !== "") {
        errorMessages += "\n\n";
      }
      errorMessages += "INVALID TARGETS\n" + errorMessages_Target;
    }

    // toggle view if helpful
    if (errorMessages_Actual === "") { toggleTargetsAndActuals(false); }
    if (errorMessages_Target === "") { toggleTargetsAndActuals(true); }

    displayError(errorMessages);
    return false;
  }
  return true;

}
function toggleTargetsAndActuals(bShowActuals, bHighlight) {
  if (bShowActuals == undefined) {
    bShowActuals = $("#metric-data-toggle-actuals").hasClass("active");
  }

  if (bShowActuals) {
    $("#import-metrics-data-btn").show();
    $("#metric-data-toggle-targets").removeClass("active");
    $("#metric-data-toggle-actuals").addClass("active");
    $(".table-metric-data").removeClass("target");
    $(".table-metric-data").addClass("actual");
    $("#explainer-target").hide();
    $("#explainer-actual").show();
  } else {
    $("#import-metrics-data-btn").hide();
    $("#metric-data-toggle-targets").addClass("active");
    $("#metric-data-toggle-actuals").removeClass("active");
    $(".table-metric-data").removeClass("actual");
    $(".table-metric-data").addClass("target");
    $("#explainer-actual").hide();
    $("#explainer-target").show();
  }

  if (bHighlight) {
    $(".explainer").highlight();
  }

  $(".table-metric-data:visible").find("input[type=text]:visible").first().focus();
}

function selectMetricDataRow(uniqueId) {
  scrollToAndHighlight($("tr[data-metric-uniqueid='" + uniqueId + "'"));
  var td = $("tr[data-metric-uniqueid='" + uniqueId + "'").find("td:nth-child(2)");
  $(td).find("input[type=text]:visible").first().focus();
}

function saveMetricData_cb(data) {
    displaySuccess("Settings successfully updated.");
}

function metricData_MoveYears(years) {
  var year = parseInt($("#year-shown").html()) + years;
  metricData_DisplayYear(year);
}

function metricData_DisplayYear(year) {
  var dataUrl = $("#data-url").val();
  dataUrl = dataUrl.replace("9999", year);
  dataUrl = dataUrl.replace("8888", $("#start-index").val());

  if ($("#metricDataList-" + year).length) {
    // already loaded, show it
    $(".table-metric-data:visible").fadeOut(400, function () {
      metricData_UpdateYearDisplay(year);
    });
  } else {
    // doesn't exist, we need to load it
    $.get(dataUrl, function (data) {
      $("#metricDataContainer").append(data);
      $(".table-metric-data:visible").fadeOut(400, function () {
        metricData_UpdateYearDisplay(year);
      });
    });
  }
}

function getMaximumDataIndex() {
  var max = 0;
  $("input[name='Data.Index']").each(function () {
    var value = parseFloat($(this).val());
    max = (value > max) ? value : max;
  });
  return max;
}

function metricData_UpdateYearDisplay(year) {
  $("#import-year").val(year);
  $("#metricDataList-" + year).show();
  $("#year-shown").html(year);
  toggleTargetsAndActuals();
  $("#start-index").val(getMaximumDataIndex() + 1);
  $(".table-metric-data:visible").find("input[type=text]:visible").first().focus();
}
