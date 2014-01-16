jQuery(function ($) {
    $(document)
        .on("change", "input[type='number'].addtocart-quantity", function () {
            var $element = $(this);
            var quantity = $element.val();
            var min = parseInt($element.attr("min") || "1");
            if ($.isNumeric(quantity)) {                
                $element.val(parseInt(quantity) > min ? quantity.toString() : min);                
            }
            else {                
                $element.val(min);
            }
        });
});