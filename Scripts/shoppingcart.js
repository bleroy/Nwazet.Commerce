jQuery(function ($) {
    var setQuantityToZero = function(parentTag) {
        return function(button) {
            return button.parents(parentTag + ":first")
                .find("input.quantity").val(0)
                .parents("form");
        };
    },
        miniLoad = function(form) {
            mini.load(form.attr("action"), form.serializeArray(), onload);
            return false;
        },
        onload = function(text) {
            mini.parent().toggle(text.trim().length > 0);
            $(this).trigger("nwazet.cartupdated");
        },
        mini = $(".minicart");

    mini.load(mini.data("url"), onload)
        .parent().hide();
    
    $(".shoppingcart .delete").live("click", function () {
        $(this).trigger("nwazet.removefromcart");
        setQuantityToZero("tr")($(this)).submit();
    });


    $(".minicart .delete").live("click", function () {
        $(this).trigger("nwazet.removefromcart");
        return miniLoad(setQuantityToZero("li")($(this)));
    });

    $(".minicart .update-button, .addtocart-button").live("click", function () {
        $(this).trigger("nwazet.addtocart");
        return miniLoad($(this).parents("form"));
    });
});