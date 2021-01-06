var autolinker;
var showingEditId;
var hideEditTimeout;


$(function () {

  autolinker = new Autolinker({ twitter: false, phone: false });


  //push metrics to the right on large screens if they are below area text
  $(".reporting-area").each(function (index) {
    var metricCount = $(this).find(".area-metric").length;
    // if there is a left-hand column for the reporting area...
    if ( $(this).find(".report-area-left-col") != null && metricCount > 1) {
      var firstMetricLeft = $(this).find(".area-metric").eq(0).position().left;
      $(this).find(".area-metric").each(function (index) {
        if ( $(this).position().left < firstMetricLeft)
        {
          //are there other metric elements on the same horizontal plane?
          var thisTop = $(this).position().top;
          var prevTop = $(this).prev().position().top;
          var nextTop = 0;
          if(index < metricCount -1){ nextTop = $(this).next().position().top; }
          if (thisTop != prevTop && thisTop != nextTop) {
            //it's riding solo, so push it right over
            $(this).addClass("col-lg-push-7");
          } else {
            //it has a neighbour, just shove it over a little
            $(this).addClass("col-lg-push-2");
          }
        };
      });
    }
  });


  $(".reporting-area-info-view").each(function (index) {
    $(this).html(autolinker.link($(this).html()));
  });

  $(".goal-info").each(function (index) {
    $(this).html(autolinker.link($(this).html()));
  });

  $(".sup-editable").each(function () {
    jQuery.data(this, "guid", guid());
    $(this).mouseover(function () {
      editorMouseover($(this));
    });
  });

  $(".edit-btn-cancel").click(function () {
    var el = $(this).closest(".sup-editable");
    closeEditor(el);
  });

  // Edit Title
  $("#edit-title").on('shown.bs.modal', function () {
    $("#Report_Title").focus();
  });

  $("#Report_Title").keypress(function (e) {
    if (e.which == 13) {
      $("form#save-report-title").submit();
      return false;
    }
    return true;
  });

  $("#save-report-title").submit(function (e) {
    var form = $(this);
    $.ajax({
      url: form.attr('action'),
      type: form.attr('method'),
      data: form.serialize(), // data to be submitted
      success: function (response) {
        $("#report-title-heading").text($("#Report_Title").val());
        $("#edit-title").modal('hide');
        $("#report-title-heading").highlight();
      },
      error: function (response) {
        displayError("An error occurred. Please try again.");
      }
    });
    return false;
  });

  // edit summary

  $(".edit-form").submit(function (e) {
    var form = $(this);
    var el = $(form).closest(".sup-editable");
    var name = $(el).data("name");
    tinyMCE.triggerSave();

    $.ajax({
      url: form.attr('action'),
      type: form.attr('method'),
      data: form.serialize(), // data to be submitted
      success: function (response) {
        var val = response.value;
        $("#" + name + "_vanilla").val(val);
        $("#" + name + "-view").html(decodeHtml(val));
        $("#" + name + "-view").html(autolinker.link($("#" + name + "-view").html()));
        closeEditor(el);
        $(el).highlight();
      },
      error: function (response) {
        displayError("An error occurred. Please try again.");
      }
    });
    return false;
  });
});



function startEdit(el) {
  var name = $(el).data("name");
  $(".sup-editable").unbind("mouseover");
  $("#" + name).val($("#" + name + "_vanilla").val());
  var h = $("#" + name + "-view").height();
  if (h < 22) h = 22;
  $("#" + name).css("min-height", h + "px");
  autosize($("#" + name));

  $("#" + name + "-view").fadeOut("fast", function () {
    $("#" + name + "-edit").fadeIn(function () {
      autosize.update($("#" + name));
      $("#" + name).focus();
    });
    $("#" + name + "-edit-buttons").slideDown();
  });
}

function editorMouseover(el) {

  if (showingEditId == $(el).data("guid")) {
    return false;
  }

  $(".sup-edit-frame").remove();
  $(".sup-editable").removeClass("sup-editable-active");
  showingEditId = $(el).data("guid");
  $(el).addClass("sup-editable-active");

  var edit_frame = $("<div/>")
    .width(el.outerWidth() + 12)
    .height(el.outerHeight() + 12)
    .addClass("sup-edit-frame")
    .css({
      "left": el.offset().left - 6,
      "top": el.offset().top - 6
    });

  var edit_btn = $("<div/>")
    .addClass("sup-edit-btn")
    .html("edit");

  if ($(el).data("type") === "modal") {
    $(edit_btn).click(function () {
      $(edit_frame).fadeOut();
      showingEditId = "";
      $("#" + $(el).data("name")).modal('show');
    });
  }

  if ($(el).data("type") === "js") {
    $(edit_btn).click(function () {
      $(edit_frame).remove();
      showingEditId = "";
      startEdit(el);
    });
  }

  $(edit_btn).appendTo(edit_frame);
  $(edit_frame).appendTo("body");
  $(edit_frame).fadeIn();
}

function closeEditor(el) {
  var name = $(el).data("name");

  $("#" + name + "-edit-buttons").slideUp();
  $("#" + name + "-edit").fadeOut("fast", function () {
    $("#" + name + "-view").fadeIn();
  });
  setTimeout(function () {
    $(".sup-editable").each(function () {
      $(this).mouseover(function () {
        editorMouseover($(this));
      });
    });
  }, 400);
}
