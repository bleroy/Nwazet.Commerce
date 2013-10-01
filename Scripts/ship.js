$(function() {
    var hasLocalStorage = function() {
        try {
            return "localStorage" in window && window.localStorage !== null;
        } catch(e) {
            return false;
        }
    },
        toggleCheckbox = $("#toggle-billing-address");
    toggleCheckbox
        .change(function() {
            $(".billing-address").toggle($(this).val());
        });
    $('input[name^="shippingAddress."]')
        .change(function () {
            if (!toggleCheckbox.prop("checked")) return;
            var input = $(this),
                name = input.attr("name").substr(16);
            $('input[name="billingAddress.' + name + '"]').val(input.val());
        });
    if (hasLocalStorage()) {
        $("input").each(function() {
            var input = $(this),
                name = input.attr("name");
            if (name && !input.val()) {
                var locallyStoredValue = localStorage[name];
                if (locallyStoredValue) {
                    input.val(locallyStoredValue).change();
                }
            }
        })
            .change(function(e) {
                var input = $(e.target),
                name = input.attr("name");
                if (name) {
                    localStorage[name] = input.val();
                }
            });
    }
});
