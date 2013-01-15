jQuery(function ($) {
    function setQuantityToZero(parentTag) {
        return function (button) {
            return button.parents(parentTag + ":first")
                .find("input.quantity").val(0)
                .parents("form");
        };
    }

    $(".shoppingcart .delete").live("click", function () {
        $(this).trigger("nwazet.removefromcart");
        setQuantityToZero("tr")($(this)).submit();
    });

    var mini = $(".minicart");
    mini.load(mini.data("url"), onload).parent().hide();

    function miniLoad(form) {
        mini.load(form.attr("action"), form.serializeArray(), onload);
        return false;
    }

    function onload(text) {
        $(this).trigger("nwazet.cartupdated");
        mini.parent().toggle(text.trim().length > 0);
    }

    $(".minicart .delete").live("click", function () {
        $(this).trigger("nwazet.removefromcart");
        return miniLoad(setQuantityToZero("li")($(this)));
    });

    $(".minicart .update-button, .addtocart-button").live("click", function () {
        $(this).trigger("nwazet.addtocart");
        return miniLoad($(this).parents("form"));
    });
});