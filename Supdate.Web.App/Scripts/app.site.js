var debug = false;
var True = true, False = false;
var emailRegex = /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$/i;
var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
var entityNiceNames = {
  area: 'Reporting Area',
  goal: 'Goal',
  metric: 'Metric',
  recipient: 'Recipient',
  teammember: 'Team Member',
  invite: 'Invite',
  attachment: 'Attachment',
  webhook: 'Webhook',
  externalapiauth: 'External API Credentials'
}
var updateDisplayOrderTimeout;
var updateDisplayOrderType;

$(function () {
  var currentUrl = window.location.pathname;

  $('input[type="file"]').prettyFile();

  $("#newAttachment").change(
    function () {
      if ($(this).val()) {
        $("#file-description-container").slideDown(
          function () {
            $("#upload-btn").fadeIn();
          });
        $("#fileDescription").focus();
      }
    }
  );

  $("form").bind("invalid-form.validate", function () {
    Ladda.stopAll();
  });

  if (typeof Ladda !== "undefined") {
    if ($(".modal").length) {
      // Hidden buttons need to be bound once modal is shown or it wont work properly
      $('.modal').on('shown.bs.modal', function () {
        Ladda.bind(".ladda-button");
        Ladda.stopAll();
      });
    }
    Ladda.bind(".ladda-button");
  }

  $($("#sidebar a").get().reverse()).each(function () {
    var myHref = $(this).attr('href');
    if ((currentUrl === myHref) || (currentUrl.substr(0, myHref.length) === myHref)) {
      $(this).addClass('active');
      $(this).closest('ul').show().closest('li').children('a').addClass('active');
      return false;
    } else {
      $(this).removeClass('active');
    }
  });

  $(".dpMonths, .dpYears").datepicker({
    autoclose: true
  });

  $("#startDate").datepicker({
    format: "MM yyyy",
    startView: "months",
    minViewMode: "months",
    autoclose: true
  });

  $("#forecastTable").niceScroll({
    styler: "fb",
    cursorcolor: "#e8403f",
    cursorwidth: '3',
    cursorborderradius: '10px',
    background: '#404040',
    spacebarenabled: false,
    cursorborder: ''
  });

  $("table[data-sortable-entity] tbody").sortable({
    helper: fixHelperModified,
    stop: updateIndex
  }).disableSelection();


  $('[data-toggle="tooltip"]').tooltip();

  $("body").on("shown.bs.modal", ".modal.modal-from-url", function (e) {
    $(this).find('input:text').first().focus();
  });

  $("body").on("hidden.bs.modal", ".modal.modal-from-url", function (e) {
    toastr.remove();
    $(this).remove();
  });

  $("body").on("click", "*[data-modal-url]", function (e) {
    e.preventDefault();
    var modal = $(this);
    modalUrl(modal.data("modal-url"));
  });

  $("body").on("click", "*[data-delete-entity]", function (e) {
    e.preventDefault();
    promptDeleteEntity($(this).data("delete-entity"), $(this).data("delete-entity-id"));
  });

  $("body").on("click", ".metric-icon", function (e) {
    e.stopPropagation();
    $($(this).data("target")).modal('show');
  });

  $("body").on("click", ".no-propagation", function (e) {
    e.stopPropagation();
  });

  $("body").on("click", "tr.select-radio", function (e) {
    e.preventDefault();
    $(this).find("input:radio").prop("checked", true);
  });

  $("body").on("change", "form :input", function () {
    $(this).closest("form").data("changed", true);
  });

  $("body").on("click", ".check-form-change", function (e) {
    if ($(this).closest("form").data("changed")) {
      var destination = $(this).attr("href");
      var promptTitle = "Un-saved Changes";
      var promptMsg = "This page has unsaved changes that will be lost if you navigate away.\n\nClick 'Confirm' to lose your changes and go to the new page.";
      var template = getConfirmNotificationTemplate("self.location.href=\"" + destination + "\"", promptTitle, promptMsg);
      displayError(template);
      centerToast();
      return false;
    }
    return true;
  });

  $("body").on("click", ".panel-plug", function (e) {
    console.log('clk');
    var url = $(this).find(".panel-footer").find("a").attr('href');
    var target = $(this).find(".panel-footer").find("a").attr('target');
    if (target == "_blank") {
      window.open(url);
    } else {
      self.location.href = url;
    }
  });

  $("button#stripe-btn").click(function() {
    $(".stripe-button-el").click();
  });

  $("button#stripe-cancel-btn").click(function () {
    cancelSubscription();
  });

  $("a[data-tweet-src]").each(function (index) {
    var msg = $($(this).data("tweet-src")).text();
    $(this).attr("target", "_blank");
    $(this).attr("href", "http://twitter.com/home/?status=" + msg.trim());
  });

  $(".dd-goto-url").change(function () {
    self.location.href = $(this).find(":selected").data("url");
  });

  $('input:checkbox.select-all').click(function (event) {
    var checkGroup = $(this).data("group");
    if (this.checked) { // check select status
      $("input:checkbox[data-group='" + checkGroup + "']").each(function () {
        this.checked = true;
      });
    } else {
      $("input:checkbox[data-group='" + checkGroup + "']").each(function () {
        this.checked = false;
      });
    }
  });

  $("a[data-hide-and-slide]").click(function (e) {
    e.preventDefault()
    var target = "#" + $(this).data("hide-and-slide");
    $(this).closest("div").fadeOut("fast", function () {
      $(target).slideDown(function () { matchNeighbourHeights(true); });
    });
  });

  $("a.view-report-recipients").click(function (e) {
    var targetHeight = 205;
    if ($(".report-date-box-h").is(":visible")) targetHeight = 225;
    if ($(this).closest(".panel").height() < targetHeight) {
      $(this).closest(".panel").height(targetHeight);
    }
  });

  $("*[data-visible-parent-width-min]").each(function (index) {
    var selector = $(this).data("parent-selector");
    if (selector == undefined) selector = "div";
    if ($(this).closest(selector).width() <= $(this).data("visible-parent-width-min")) {
      $(this).hide();
    } else {
      $(this).show();
    }
  });

  $("*[data-visible-parent-width-max]").each(function (index) {
    var selector = $(this).data("parent-selector");
    if (selector == undefined) selector = "div";
    if ($(this).closest(selector).width() > $(this).data("visible-parent-width-max")) {
      $(this).hide();
    } else {
      $(this).show();
    }
  });


  $("table.remove-empty-columns th").each(function (i) {
    var remove = 0;

    var tds = $(this).parents("table").find("tr td:nth-child(" + (i + 1) + ")");
    tds.each(function (j) { if ($.trim(this.innerHTML) == "") {remove++;} });

    if (remove == ($(this).parents("table").find("tr").length - 1)) {
      $(this).hide();
      tds.hide();
    }
  });

  $("#splash").click(function () {
    $("#splash").fadeOut("fast");
  });

  if (getUrlVars()["splash"] === "1") {
    runSplash();
  }

  $("tr.row-clickable td.mover").click(function (e) {
    e.stopPropagation();
  });

  $("tr.row-clickable td a.helper").click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).closest("tr.row-clickable").trigger("click");
  });
  $("td.row-clickable a.helper").click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).closest("td.row-clickable").trigger("click");
  });

  $("body").on("click", ".modal-content button.ajax-submit", function (event) {
    $(this).closest(".ajax-form").submit();
  });

  $("body").on("submit", ".ajax-form", function (event) {
    event.preventDefault();
    if ($(this).attr("data-processing") != "true") {
      $(this).attr("data-processing", true);
      toastr.remove();
      var cbFunction = $(this).find(".ajax-submit").data("success-callback");
      var modal = $(this).closest(".modal");
      var modalId = $(modal).attr("id");
      var theForm = $(this);

      theForm.removeData('validator');
      theForm.removeData('unobtrusiveValidation');
      $.validator.unobtrusive.parse($(theForm));

      var ajaxOptions = {
        data: $(theForm).serialize()
      }

      if ($(theForm).attr("enctype") == "multipart/form-data") {
        ajaxOptions = {
          data: new FormData($(this)[0]),
          contentType: false,
          async: false,
          cache: false,
          processData: false,
        }
      }

      if ($(theForm).valid()) {
        $.ajax($.extend(
        {
          type: "POST",
          url: $(theForm).attr("action"),
          success: function(msg) {
            $(theForm).attr("data-processing", "false");
            if (msg.success) {
              Ladda.stopAll();
              if (typeof window[cbFunction] == "function")
                window[cbFunction].call(null, msg, modalId);
            } else {
              displayErrorsFromJson(msg);
            }
          },
          error: function() {
            $(theForm).attr("data-processing", "false");
            displayError("An error occurred. Please try again.");
          }
        }, ajaxOptions));
      } else {
        // remove processing tag from invalid form
        $(this).attr("data-processing", false);
      }
    } else {
      console.log('Not processing form, already processing');
    }
  });

  tinymce.baseURL = "/Assets/tinymce";
  tinymce.suffix = ".min";
  tinymce.init({
      selector: ".tinymce-basic",
      menubar: false,
      statusbar:false,
      branding: false,
      elementpath: false,
      encoding: "xml",
      plugins: "lists link textcolor code paste",
      paste_auto_cleanup_on_paste: true,
      toolbar: ["fontselect fontsizeselect forecolor | bold italic underline ", "| link | bullist numlist | alignleft aligncenter alignright alignjustify | indent outdent blockquote | code"],
      setup: function (editor) {
          editor.on('SaveContent', function (ed) {
              ed.content = ed.content.replace(/&#39/g, "&apos");
          });
      }
  });

  setFaAutoMimeTypes();

  $(window).load(function () {
    matchNeighbourHeights();
  });
  $(window).resize(function () {
    matchNeighbourHeights(true);
  });

  // For notification messages.
  displayMessage();
});

var fixHelperModified = function(e, tr) {
  var $originals = tr.children();
  var $helper = tr.clone();
  $helper.children().each(function(index) {
    $(this).width($originals.eq(index).width());
  });

  return $helper;
};

var updateIndex = function(e, ui) {
  $('td.index', ui.item.parent()).each(function(i) {
    $(this).html(i + 1);
  });
  updateDisplayOrderType = ui.item.closest("table").attr("data-sortable-entity");
  clearTimeout(updateDisplayOrderTimeout)
  updateDisplayOrderTimeout = setTimeout(function() { updateDisplayOrder(); }, 3000);
};

function setFaAutoMimeTypes(){
$(".fa-auto-mimetype").each(function (index) {
  $(this).addClass(mimeTypeToFontAwesome($(this).attr("data-mimetype")));
  $(this).removeClass('fa-auto-mimetype');
});
}
function updateDisplayOrder(row) {
  var displayOrder = [];
  $("table[data-sortable-entity='" + updateDisplayOrderType + "'] > tbody  > tr").each(function (index) {
    displayOrder.push({
      "EntityId": $(this).data(updateDisplayOrderType + "-id"),
      "DisplayOrder": index
    });
  });
  $.post(displayOrderUrls[updateDisplayOrderType], "displayOrder=" + JSON.stringify(displayOrder), function (data) {
    // Display order saved.
  });
}
function matchNeighbourHeights(reset) {
  if (reset) {
    $("div[data-match-neighbour-height]").attr('data-match-neighbour-height', 'true');
    $("div[data-match-neighbour-height]").attr('data-height-group-offset', '');
  }
  var targetHeight = 0;
  var els = $("div[data-match-neighbour-height='true']");
  if (els.length === 0) {
    verticallyCenter();
    return;
  }
  var targetTop = els.first().offset().top;
  var i = 0;
  $("div[data-match-neighbour-height='true']").each(function () {
    if ($(this).offset().top === targetTop) {
      var thisHeight = $(this).height();
      if (thisHeight > targetHeight) targetHeight = thisHeight;
      $(this).attr("data-height-group-offset", targetTop);
    }
  });
  $("div[data-height-group-offset='" + targetTop + "']").height(targetHeight);
  $("div[data-height-group-offset='" + targetTop + "']").attr('data-match-neighbour-height', 'done');
  matchNeighbourHeights();
}
function verticallyCenter() {
  var els = $("*[data-vertical-center='true']");
  if (els.length === 0) return;
  $("*[data-vertical-center='true']").each(function () {
    var parentHeight = $(this).parent().innerHeight();
    var thisHeight = $(this).height();
    if (thisHeight < parentHeight) {
      var margin = (parentHeight - thisHeight) / 2;
      $(this).css("margin-top", margin + "px");
    }
    $(this).css("visibility", "visible");
  });
}
function changeCompany(companyGuid, successUrl) {
  if (successUrl == undefined) {
    successUrl = miscUrls.showDashboard + "?splash=1";
  }
  $.ajax({
    type: "POST",
    url: miscUrls.switchCompany,
    data: { companyUniqueId: companyGuid },
    success: function (result) {
      if (result.success) {
        window.location = successUrl;
      }
    },
    error: function () {
      displayError("An error occurred while trying to switch companies. Please try again.");
    }
  });
}

function runSplash() {
  $("#splash").show(0, function () {
    setTimeout(function () { $("#splash").fadeOut("slow"); }, 1500);
  });
}

function displayError(message) {
  //re-enable any Ladda buttons
  setTimeout(function () { Ladda.stopAll(); }, 1000);

  displayMessage(0, message);
}

function displayWarning(message) {
  displayMessage(1, message);
}

function displaySuccess(message) {
  displayMessage(2, message);
}

function displayStickySuccess(message) {
  displayMessage(3, message);
}

function displayMessage(messageType, message) {
  if (message === undefined) {
    message = $("#notificationMessage").html();
  }

  if (message && message.length > 0) {
    var notificationType = "info";

    if (messageType === undefined) {
      messageType = $("#notificationType").html();
    }

    if (messageType == 0) {
      notificationType = "error";
    }
    else if (messageType == 1) {
      notificationType = "warning";
    }
    else if (messageType == 2 || messageType == 3) {
      notificationType = "success";
    }

    if (messageType == 0 || messageType == 1 || messageType == 3) {
      toastr.options.timeOut = 0;
      toastr.options.extendedTimeOut = 0;
      toastr.options.closeButton = true;
    }

    toastr.options.positionClass = "toast-top-center pre-wrap";
    toastr.options.progressBar = true;
    toastr[notificationType](message);
  }
}

function validateEmailAddress(emailAddress, maxLength) {
  if (emailAddress === "") {
    return "Email address is required.\n";
  }
  else if (maxLength != null && emailAddress.length > maxLength) {
    return "Email address cannot be longer than " + maxLength + " characters.\n";
  }
  else if (!emailRegex.test(emailAddress)) {
    return "Invalid email address.\n";
  }

  return "";
}

function convertJsonDateToDate(jsonDate) {
  if (jsonDate != null) {
    return new Date(parseInt(jsonDate.substr(6, jsonDate.lastIndexOf(")"))));
  }
  return null;
}

function getConfirmNotificationTemplate(returnFunction, _templateTitle, _templateMessage) {
  if (_templateTitle == undefined) { _templateTitle = "Are you sure?"; }
  if (_templateMessage == undefined) { _templateMessage = "Are you sure you want to delete this record?"; }
  var template = parseTemplate($("#confirmNotificationTemplate").html(), {
    returnFunction: "return " + returnFunction,
    templateTitle: _templateTitle,
    templateMessage: _templateMessage
  });

  return template;
}


function resizeGhost(event, ui) {
  var helper = ui.helper;
  var element = $(event.target);
  helper.width(element.width());
  helper.height(element.height());
}

function flashTip(idString, _placement, _title, delay, duration) {
  // create tooltip
  $(idString).tooltip({ placement: _placement, title: _title });

  // display it
  setTimeout(function () {
    $(idString).tooltip('show');
  }, delay);

  // hide and destroy
  setTimeout(function () {
    $(idString).tooltip('hide');
    setTimeout(function () {
      $(idString).tooltip('destroy');
    }, 1000);
  }, duration);

}

function cancelSubscription() {
  var promptTitle = "Cancel Subscription";
  var promptMsg = "Please confirm you want to cancel your subscription.\n\nYou will no longer be able to access premium features.";
  var template = getConfirmNotificationTemplate("cancelSubscription_confirmed()", promptTitle, promptMsg);
  displayError(template);
  centerToast();
}
function cancelSubscription_confirmed() {
  $("#stripe-cancel-subscription-form").submit();
}
function mimeTypeToFontAwesome(mimeType) {
  switch (mimeType) {
    case 'image/jpeg':
      return 'fa-picture-o';
    case 'image/jpg':
      return 'fa-picture-o';
    case 'image/png':
      return 'fa-picture-o';
    case 'text/html':
      return 'fa-file-text-o';
    case 'application/pdf':
      return 'fa-file-pdf-o';
    default:
      return 'fa-file-o';
  }
}

function modalUrl(url, isStatic) {
  var modalId = "modal_" + guid();
  $("#" + modalId).remove();

  var modalDiv = $("<div class='modal fade modal-from-url' id='" + modalId + "'></div>");
  $(modalDiv).html("<div class='modal-dialog'><div class='modal-content'></div></div>");
  $("body").append(modalDiv);

  var modalAttributes = {
    show: true,
    remote: url
  };

  if (isStatic) {
    modalAttributes["modal"] = true;
    modalAttributes["backdrop"] = 'static';
    modalAttributes["keyboard"] = false;
  }

  $("#" + modalId).modal(modalAttributes);
}

function getUrlVars() {
  var vars = [], hash;
  var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
  for (var i = 0; i < hashes.length; i++) {
    hash = hashes[i].split('=');
    vars.push(hash[0]);
    vars[hash[0]] = hash[1];
  }

  return vars;
}
function createCompany_cb(data) {
  var url = [location.protocol, '//', location.host, location.pathname].join("");
  window.location.href = url + "?company-added=1";
}
function importMetricData_cb(data) {
  if (data.success) {
    var updatedCount = 0;
    var results = data.results;
    for (var i = 0; i < results.length; i++) {
      var locator = results[i].Date + "-" + results[i].MetricId;
      var el = $("input[data-metricdatapoint-locator=" + locator + "]");
      if (el.length) {
        if (results[i].Actual != 0 || $(el).val() != "") {
          $(el).val(results[i].Actual);
          $(el).highlight();
          if ($(el).hasClass("actual")) {
            $(el).closest("td").addClass("imported");
          } else {
            $(el).addClass("imported");
          }
          updatedCount++;
        }
      }
    }
    if (updatedCount > 0) {
      displaySuccess(updatedCount + " values have been imported.\nClick 'Save' after reviewing them.");
      flashTip("#save-metric-data", "top", "Remember to click Save after you've\nreviewed the imported data.", 3500, 12000);

    }
    displayErrorsFromJson(data);
  }
}
function manageModalValidation(form) {
  var validator = form.validate({ focusout: false, keyup: false });
  // get errors that were created using jQuery.validate.unobtrusive
  var $errors = form.find(".field-validation-error span");

  // trick unobtrusive to think the elements were successfully validated
  // this removes the validation messages
  errors.each(function () { validator.settings.success($(this)); });

  // clear errors from validation
  validator.resetForm();
}

function TestExternalApiCredentials() {
  $("#credentials-test-result-passed").hide();
  $("#credentials-test-result-failed").hide();
  $("#credentials-test-result-wait").show();

  $("#TestToken").val($("#Token").val());
  $("#TestKey").val($("#Key").val());
  $("#credentialsTestForm").submit();

}
function TestExternalApiCredentials_cb(data) {
  if (data.result) {
    $("#credentials-test-result-wait").hide();
    $("#credentials-test-result-failed").hide();
    $("#credentials-test-result-passed").fadeIn();
  } else {
    $("#credentials-test-result-wait").hide();
    $("#credentials-test-result-passed").hide();
    $("#credentials-test-result-failed").fadeIn();
  }
}

function setDDLOptionsTextToDataValue(ddl, dataVal) {
  $('select' + ddl + " > option").each(function () {
    if ($(this).attr("data-" + dataVal)) {
      ($(this).text($(this).data(dataVal)));
    }
  });
}
function updatePreviewData() {
  var emailField = $("#EmailField").val();
  var firstNameField = $("#FirstNameField").val();
  var lastNameField = $("#LastNameField").val();
  $(".import-error").removeClass("import-error");
  toastr.clear();

  $("#imported_data tbody tr").each(function () {
    var idx = $(this).data("index");

    // get the selected values
    var email = $("#Values_" + idx + "_" + emailField).val();
    var firstName = "";
    var lastName = "";
    if (firstNameField >= 0) {
      firstName = $("#Values_" + idx + "_" + firstNameField).val();
    }
    if (lastNameField >= 0) {
      lastName = $("#Values_" + idx + "_" + lastNameField).val();
    }

    // store the values
    $("#Data_" + idx + "_Email").val(email);
    $("#Data_" + idx + "_FirstName").val(firstName);
    $("#Data_" + idx + "_LastName").val(lastName);

    // display the values
    $("#DisplayEmail_" + idx).text(email);
    var displayName = firstName;
    if (displayName != "" && lastName != "") displayName += " ";
    displayName += lastName;
    $("#DisplayName_" + idx).text(displayName);
  });
}
function guid() {
  function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
      .toString(16)
      .substring(1);
  }
  return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}

function displayErrorsFromJson(json) {
  if (json.error) {
    displayError(json.error);
    return;
  }
  if (json.errors) {
    json.errors.forEach(function (obj) {
      displayError(obj.ErrorMessage);;
    });
    return;
  }
  displayError("An unknown error occured. Please check your data and try again.");
}
function formatNumber(prefix, suffix, thousandsSeparator, value) {
  if (thousandsSeparator) value = addCommas(value);
  return ValueOrNull(prefix,'') + value + ValueOrNull(suffix,'');
}
function ValueOrNull(val, ifNull) {
  if (val) return val;
  return ifNull;
}
function addCommas(nStr) {
  nStr += '';
  x = nStr.split('.');
  x1 = x[0];
  x2 = x.length > 1 ? '.' + x[1] : '';
  var rgx = /(\d+)(\d{3})/;
  while (rgx.test(x1)) {
    x1 = x1.replace(rgx, '$1' + ',' + '$2');
  }
  return x1 + x2;
}
function reloadPage() {
  window.location.reload();
}
function setCookie(cname, cvalue, exdays) {
  var d = new Date();
  d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
  var expires = "expires=" + d.toUTCString();
  document.cookie = cname + "=" + cvalue + "; " + expires;
}
function getCookie(cname) {
  var name = cname + "=";
  var ca = document.cookie.split(';');
  for (var i = 0; i < ca.length; i++) {
    var c = ca[i];
    while (c.charAt(0) == ' ') c = c.substring(1);
    if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
  }
  return "";
}
function debugLog(s) {
  if (debug) console.log(s);
}

function scrollToAndHighlight(el) {

  var afterScroll = function (el) {
     $(el).highlight();
  }

  if (!isElementInViewport(el)) {
    $('html, body').animate({
      scrollTop: $(el).offset().top - ($(window).height() - $(el).outerHeight() - 100)
    }, 500, "linear", afterScroll());
  } else {
    afterScroll(el);
  }
}

function isElementInViewport(el) {
  //special bonus for those using jQuery
  if (typeof jQuery === "function" && el instanceof jQuery) {
    el = el[0];
  }
  if (el == undefined) return false;

  var rect = el.getBoundingClientRect();

  return (
      rect.top >= 0 &&
      rect.left >= 0 &&
      rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) && /*or $(window).height() */
      rect.right <= (window.innerWidth || document.documentElement.clientWidth) /*or $(window).width() */
  );
}

function decodeHtml(html) {
    return $('<div>').html(html).text();
}
