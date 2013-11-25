$(function() {
    var hasLocalStorage = function() {
        try {
            return "localStorage" in window && window.localStorage !== null;
        } catch(e) {
            return false;
        }
    },
        addressForm = $("#address-form"),
        errorZone = $(".ship-errors"),
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

    addressForm.find(".required").after(
        $("<span class='error-indicator' title='" + required + "'>*</span>"));
    addressForm.submit(function (e) {
        var validated = true,
            firstErrorElement,
            alreadyRequired = [];
        addressForm.find(".required").each(function () {
            var requiredField = $(this);
            if (!requiredField.val()) {
                validated = false;
                var id = requiredField.attr("id"),
                    label = addressForm.find("label[for='" + id + "']").html();
                requiredField.addClass("required-error");
                if (alreadyRequired.indexOf(label) == -1) {
                    errorZone.show().append(
                        $("<div></div>").html(requiredFormat.replace("{0}", label))
                    );
                    alreadyRequired.push(label);
                }
                if (!firstErrorElement) {
                    firstErrorElement = this;
                    firstErrorElement.focus();
                }
            } else {
                requiredField.removeClass("required-error");
            }
        });
        if (!validated) {
            e.preventDefault();
            if (firstErrorElement) {
                firstErrorElement.scrollIntoView();
            }
            return false;
        }
    });
});
