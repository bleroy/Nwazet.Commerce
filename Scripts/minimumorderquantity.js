jQuery(function ($) {
    $(document)
        .on("change", "input[type='number'].addtocart-quantity", function () {
            var $element = $('input[type="number"]', $(this).parent());
            var quantity = $element.val();
            if ($.isNumeric(quantity)) {
                if ($element.attr("min") != null) {
                    if (quantity > parseInt($element.attr("min"), 0)) {
                        $element.val(parseInt(quantity.toString(), 0));
                    }
                    else {
                        $element.val(parseInt($element.attr("min"), 0));
                    }
                }
            }
            else {
                if ($element.attr("min") != null) {
                    $element.val(parseInt($element.attr("min"), 0));
                }
            }
        });
});