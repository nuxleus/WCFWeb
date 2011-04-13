(function ($) {
    $.fn.extend({
        starRatings: function (init) {
            var $thisObj = this;

            return $thisObj.each(function () {
                $stars = $("select option", this);
                $stars.nextAll().each(function () {
                    $thisObj.append($stars.index(this) <= init ? "<td class='star-rating-element star-rating-element-selected'/>" : "<td class='star-rating-element'/>");
                });
                var $starsCtrl = $(".star-rating-element", $thisObj).click(click).mouseenter(mouseenter).mouseleave(mouseleave);

                function click() {
                    $(this).nextAll().removeClass('star-rating-element-selected').end().prevAll().andSelf().addClass('star-rating-element-selected');
                    $thisObj.triggerHandler("ratingChanged", { rating: $starsCtrl.index(this) + 1 });
                }
                function mouseenter() { $(this).prevAll().andSelf().addClass('star-rating-element-hover'); }
                function mouseleave() { $(this).prevAll().andSelf().removeClass('star-rating-element-hover'); }
            });
        }
    });
})(jQuery);
