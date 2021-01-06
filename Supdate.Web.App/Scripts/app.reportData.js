$(document).ready(function () {
  $.getJSON(graphDataGetUrl, function (chartData) {
      bindMetricChart(chartData);
    });
});