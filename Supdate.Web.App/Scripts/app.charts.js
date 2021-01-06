var storedChartData;

function loadGraph(chartData, targetDivId, chartStyle, chartType) {
  // defaults (line chart, non-thumb)
  var _ykeys;
  var _labels;
  var _events = [];
  var _hideHover = false;
  var _grid = true;
  var _pointSize = 4;
  var _resize = true;

  var morrisChartType = Morris.Line;
  if (dataHasTargets(chartData) && chartStyle != 'thumb') {
    _ykeys = ['Actual', 'Target'];
    _labels = ['Actual', 'Target'];
  } else {
    _ykeys = ['Actual'];
    _labels = [''];
  }

  if (chartType == 'Bar Graph') {
    morrisChartType = Morris.Bar;
  }

  if (chartStyle == 'thumb') {
    _hideHover = 'always';
    _grid = false;
    _pointSize = 0;
    _resize = false;
  }

  new morrisChartType({
    element: targetDivId,
    parseTime: false,
    data: chartData,
    xkey: 'DisplayName',
    ykeys: _ykeys,
    labels: _labels,
    hideHover: _hideHover,
    grid: _grid,
    pointSize: _pointSize,
    resize: _resize,
    hoverCallback: function (index, options, content) {
      var thisData = options.data[index];
      var labelTitle = "<div class='morris-hover-row-label' style='color: #f26622;'>" + options.data[index].DisplayNameLong + "</div>";
      var formattedActual = formatNumber(thisData.Prefix, thisData.Suffix, true, thisData.Actual);
      if (options.labels.length > 1 && thisData.Target != null) {
        // has target value
        var formattedTarget = formatNumber(thisData.Prefix, thisData.Suffix, true, thisData.Target);
        var html = "";
        if (thisData.Actual != null) {
          html = "<div class='morris-hover-point' style='color: #0b62a4; font-size:1.4em;'>" + formattedActual + "</div>";
        }
        html = html +  "<div class='morris-hover-point' style='color: #7A92A3;'>Target: " + formattedTarget + "</div>";
        return labelTitle +  html;
      } else {
        // no target value
        var html =  "<div class='morris-hover-point' style='color: #0b62a4; font-size:1.4em;'>" + formattedActual + "</div>";
        return labelTitle +  html;
      };
    }
  });
}
function dataHasTargets(data) {
  for (var i = 0; i < data.length; i++) {
    if (data[i].Target) {
      return true;
    }
  }
  return false;
}
function bindMetricChart(chartData) {
  storedChartData = chartData;

  $.each(chartData, function (key, data) {
    var metricId = data.UniqueId;
    var chartTypeValue = $("#chart-type-" + metricId).val();
    var chartId = "chart-" + metricId;
    var chartModalId = "#chart-" + metricId + "-modal";
    var chartFullId = "chart-" + metricId + "-full";

    //get start and end dates for thumbnail and default full graph to these dates
    var thumbStartDate = new Date(data.ThumbnailStartDate);
    var thumbEndDate = new Date(data.ThumbnailEndDate);
    var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    $(chartModalId + "-start-date").val(monthNames[thumbStartDate.getMonth()] + " " + thumbStartDate.getFullYear());
    $(chartModalId + "-end-date").val(monthNames[thumbEndDate.getMonth()] + " " + thumbEndDate.getFullYear());
    $(chartModalId + "-startDate").data("properDate", thumbStartDate);
    $(chartModalId + "-endDate").data("properDate", thumbEndDate);

    if (chartTypeValue !== undefined && chartTypeValue !== "No Graph") {
      if (data.ThumbnailGraph.length < 2 && chartTypeValue == "Line Chart") {
        // line graph with single data point
        dottyHtml = "<i class='fa fa-circle'></i>";
        $("#" + chartId).html(dottyHtml);
      } else {
        loadGraph(data.ThumbnailGraph, chartId, "thumb", chartTypeValue);
        $("#chart-type-" + metricId).parent().removeClass("noThumbnail");
      }

      var dataStartDate = new Date(data.StartDate);
      var dataEndDate = new Date(data.EndDate);

      $(chartModalId).on("shown.bs.modal", function () {
        displayFullChart(metricId, chartModalId, chartFullId, chartTypeValue);

        $(chartModalId + "-startDate").datepicker({
          format: "M yyyy",
          startView: "months",
          minViewMode: "months",
          autoclose: true,
          startDate: dataStartDate,
          endDate: dataEndDate
        }).on("changeDate", function (e) {
          if (e.date) {
            $(this).data("properDate", e.date);
            displayFullChart(metricId, chartModalId, chartFullId, chartTypeValue);
          }
        }).on("show", function (e) {
          if (e.date) {
            $(this).data("stickyDate", e.date);
          }
          else {
            $(this).data("stickyDate", null);
          }
        }).on("hide", function (e) {
          var stickyDate = $(this).data("stickyDate");

          if (!e.date && stickyDate) {
            $(this).datepicker('setDate', stickyDate);
            $(this).data("stickyDate", null);
          }
        });

        $(chartModalId + "-endDate").datepicker({
          format: "M yyyy",
          startView: "months",
          minViewMode: "months",
          autoclose: true,
          startDate: dataStartDate,
          endDate: dataEndDate,
        }).on("changeDate", function (e) {
          if (e.date) {
            $(this).data("properDate", e.date);
            displayFullChart(metricId, chartModalId, chartFullId, chartTypeValue);
          }
        }).on("show", function (e) {
          if (e.date) {
            $(this).data("stickyDate", e.date);
          }
          else {
            $(this).data("stickyDate", null);
          }
        }).on("hide", function (e) {
          var stickyDate = $(this).data("stickyDate");

          if (!e.date && stickyDate) {
            $(this).datepicker("setDate", stickyDate);
            $(this).data("stickyDate", null);
          }
        });

      });

      $(chartModalId).on("hidden.bs.modal", function () {
        $("#" + chartFullId).empty();
      });
    }
  });

  $("td.noThumbnail").html("");
}

function displayFullChart(metricId, chartModalId, chartFullId, chartTypeValue) {
  $("#" + chartFullId).empty();
  for (var i = 0; i < storedChartData.length; i++) {
    var chartGraph = storedChartData[i];
    if (chartGraph.UniqueId == metricId) {
        console.log('Displaying Chart Id: ' + chartGraph.UniqueId);
        console.log('Data Start Date: ' + chartGraph.StartDate);
        console.log('Data End Date: ' + chartGraph.EndDate);
      var startDate = new Date($(chartModalId + "-startDate").data("properDate"));
      var endDate = new Date($(chartModalId + "-endDate").data("properDate"));
      startDate.setHours(0, 0, 0, 0);
      endDate.setHours(0, 0, 0, 0);
      console.log('User Start Date: ' + startDate);
      console.log('User End Date: ' + endDate);
      var filteredData = [];
      var included = 0;
      var excluded = 0;
      for (var x = 0; x < chartGraph.FullGraph.length; x++) {
        var dataPoint = chartGraph.FullGraph[x];
        var dataPointDate = new Date(dataPoint.DateIso);
        dataPointDate.setHours(0, 0, 0, 0);
        if (dataPointDate >= startDate && dataPointDate <= endDate) {
          filteredData.push(dataPoint);
          included++;
        } else {
          excluded++;
        }
      }
      console.log('Included: ' + included);
      console.log('Excluded: ' + excluded);
      loadGraph(filteredData, chartFullId, null, chartTypeValue);
    }
  }
}
