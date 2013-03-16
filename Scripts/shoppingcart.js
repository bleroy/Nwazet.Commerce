jQuery(function($) {
    var mini = $(".minicart"),
        setQuantityToZero = function(parentTag) {
        return function(button) {
            if (findNextIndex(button.closest("form")) == 1 && hasLocalStorage()) {
                localStorage["nwazet.cart"] = "";
            }
            return button.closest(parentTag)
                .find("input.quantity").val(0)
                .closest("form");
        };
    },
        miniLoad = function (form) {
            mini.load(form.attr("action"), form.serializeArray(), onload);
            $(this).trigger("nwazet.cartupdating");
            return false;
        },
        hasLocalStorage = function() {
            try {
                return "localStorage" in window && window.localStorage != null;
            } catch(e) {
                return false;
            }
        },
        onload = function(text) {
            var gotCart = $.trim(text).length > 0;
            if (hasLocalStorage()) {
                if (gotCart) {
                    localStorage["nwazet.cart"] = text;
                } else {
                    var localCart = localStorage["nwazet.cart"];
                    if (localCart) {
                        mini.html(localCart);
                        miniLoad(mini.find("form"));
                    }
                }
            }
            mini.parent().toggle(gotCart);
            $(this).trigger("nwazet.cartupdated");
        },
        findNextIndex = function(form) {
            var maxIndex = -1;
            if (form) {
                var formData = form.serializeArray();
                $.each(formData, function() {
                    var name = this.name;
                    if (name.substr(0, 6) == "items[" && name.slice(-11) == "].ProductId") {
                        maxIndex = Math.max(maxIndex, +name.slice(6, name.length - 11));
                    }
                });
                maxIndex++;
            }
            return maxIndex;
        };

    mini.load(mini.data("url"), onload)
        .parent().hide();

    $(document)
        .on("click", ".shoppingcart .delete", function() {
            $(this).trigger("nwazet.removefromcart");
            setQuantityToZero("tr")($(this)).submit();
        })
        .on("click", ".minicart .delete", function() {
            $(this).trigger("nwazet.removefromcart");
            return miniLoad(setQuantityToZero("li")($(this)));
        })
        .on("click", ".minicart .update-button", function() {
            return miniLoad($(this).closest("form"));
        })
        .on("submit", "form.addtocart", function(e) {
            $(this).trigger("nwazet.addtocart");
            e.preventDefault();
            var addForm = $(this),
                addFormData = addForm.serializeArray(),
                minicartForm = mini.find("form"),
                productId = addForm.data("productid"),
                quantity = addForm.find("input[name=\"quantity\"]").val(),
                inputTag = "<input type=\"hidden\"/>",
                maxIndex = findNextIndex(minicartForm),
                attrIndex = 0,
                prefix = "items[" + maxIndex + "].";
            if (minicartForm.length != 0) {
                minicartForm
                    .append($(inputTag).attr({
                        name: prefix + "ProductId",
                        value: productId
                    }))
                    .append($(inputTag).attr({
                        name: prefix + "Quantity",
                        value: quantity
                    }));
                $.each(addFormData, function() {
                    var name = this.name;
                    if (name.substr(0, 18) == "productattributes[" && name.slice(-5) == "].Key") {
                        var index = name.slice(18, name.length - 5),
                            value = $.grep(addFormData, function(element) {
                                return element.name == "productattributes[" + index + "].Value";
                            })[0].value;
                        if (value != "__none__") {
                            var attributePrefix = prefix + "AttributeIdsToValues[" + attrIndex++ + "].";
                            minicartForm
                                .append($(inputTag).attr({
                                    name: attributePrefix + "Key",
                                    value: this.value
                                }))
                                .append($(inputTag).attr({
                                    name: attributePrefix + "Value",
                                    value: value
                                }));
                        }
                    }
                });
                miniLoad(minicartForm);
            } else {
                miniLoad(addForm);
            }
        });
});