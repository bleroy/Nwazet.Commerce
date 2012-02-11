jQuery(function ($) {
    function setQuantityToZero(parentTag) {
        return function (button) {
            return button.parents(parentTag + ":first")
                .find("input.quantity").val(0)
                .parents("form");
        };
    }

    $(".shoppingcart .delete").live("click", function () {
        setQuantityToZero("tr")($(this)).submit();
    });

    var mini = $(".minicart");
    mini.load(mini.data("url"));

    function miniLoad(form) {
        mini.load(form.attr("action"), form.serializeArray());
        return false;
    }

    $(".minicart .delete").live("click", function () {
        return miniLoad(setQuantityToZero("li")($(this)));
    });

    $(".minicart .update-button, .addtocart-button").live("click", function () {
        return miniLoad($(this).parents("form"));
    });
});