jQuery.fn.highlight = function () {
  $(this).each(function () {
    var el = $(this);
    $("<div/>")
    .width(el.outerWidth() + 12)
    .height(el.outerHeight() + 12)
    .css({
      "position": "absolute",
      "left": el.offset().left - 6,
      "top": el.offset().top - 6,
      "background-color": "#ffff99",
      "opacity": ".7",
      "z-index": "9999999",
      "border-radius": "6px"
    }).appendTo('body').fadeOut(1000).queue(function () { $(this).remove(); });
  });
}
