jQuery(function($) {
    var hasLocalStorage = function() {
            try {
                return "localStorage" in window && window.localStorage !== null;
            } catch(e) {
                return false;
            }
        },
        setLoading = function(state) {
            if (hasLocalStorage()) {
                localStorage["nwazet-cart-loading"] = !!state;
            }
            return loading = !!state;
        },
        loading = hasLocalStorage() ? localStorage["nwazet-cart-loading"] == "true" : false,
        nwazetCart = "nwazet.cart",
        cartContainer = $(".shopping-cart-container"),
        setQuantityToZero = function(parentTag) {
            return function(button) {
                if (findNextIndex(button.closest("form")) === 1 && hasLocalStorage()) {
                    localStorage.removeItem(nwazetCart);
                }
                return button.closest(parentTag)
                    .find("input.quantity").val(0)
                    .closest("form");
            };
        },
        cartContainerLoad = function (form) {
            if (!loading && form && form.length > 0) {
                setLoading(true);
                cartContainer.load(form[0].action || updateUrl, form.serializeArray(), onCartLoad);
                $(this).trigger("nwazet.cartupdating");
            }
            return false;
        },
        buildForm = function(state, container) {
            $.each(state, function (key, value) {
                if (key !== "__RequestVerificationToken") {
                    container.append($("<input type='hidden'/>")
                        .attr("name", key)
                        .val(value));
                }
            });
            return container;
        },
        notify = function(text) {
            $("#shopping-cart-notification")
                .html(text)
                .show();
            console.log(text);
        },
        onCartLoad = function (text, status) {
            $("#shopping-cart-notification").hide();
            if (status === "error") {
                notify(window.Nwazet.FailedToLoadCart);
            } else {
                var gotCart = text === false ||
                    (typeof (text) === "string" && $.trim(text).length > 0);
                if (hasLocalStorage()) {
                    if (gotCart) {
                        var form = cartContainer.find("form");
                        if (form.length === 0) {
                            form = cartContainer.closest("form");
                        }
                        if (form.length !== 0 && form[0].length > 1) {
                            var cartArray = form.serializeArray(),
                                cart = {};
                            $.each(cartArray, function(index, formField) {
                                cart[formField.name] = formField.value;
                            });
                            delete cart.__RequestVerificationToken;
                            localStorage[nwazetCart] = JSON.stringify(cart);
                        }
                        setLoading(false);
                    } else {
                        var cachedCart, cachedCartString = localStorage[nwazetCart];
                        if (cachedCartString) {
                            if (loading) {
                                localStorage[nwazetCart] = JSON.stringify({
                                    Country: localStorage[nwazetCart].Country || null,
                                    ZipCode: localStorage[nwazetCart].ZipCode || null
                                });
                                setLoading(false);
                                return;
                            }
                            try {
                                cachedCart = JSON.parse(cachedCartString);
                            } catch(ex) {
                                localStorage.removeItem(nwazetCart);
                                return;
                            }
                            var cartContainerForm = cartContainer.closest("form");
                            if (cartContainerForm.length === 0) {
                                cartContainerForm = $("<form></form>")
                                    .append($("<input name='__RequestVerificationToken'/>").val(token));
                            }
                            buildForm(cachedCart, cartContainerForm);
                            notify(window.Nwazet.WaitWhileWeRestoreYourCart);
                            if (cartContainer.hasClass("minicart")) {
                                cartContainerLoad(cartContainerForm);
                            } else {
                                setLoading(true);
                                cartContainer.closest("form").submit();
                                return;
                            }
                        }
                    }
                }
                if (cartContainer.hasClass("minicart")) {
                    cartContainer.parent().toggle(gotCart);
                } else {
                    setLoading(false);
                }
                $(this).trigger("nwazet.cartupdated");
            }
        },
        findNextIndex = function(form) {
            var maxIndex = -1;
            if (form) {
                var formData = form.serializeArray();
                $.each(formData, function() {
                    var name = this.name;
                    if (name.substr(0, 6) === "items[" && name.slice(-11) === "].ProductId") {
                        maxIndex = Math.max(maxIndex, +name.slice(6, name.length - 11));
                    }
                });
                maxIndex++;
            }
            return maxIndex;
        },
        loadUrl = cartContainer.data("load"),
        updateUrl = cartContainer.data("update"),
        token = cartContainer.data("token");

    if (loadUrl) {
        cartContainer
            .load(loadUrl, onCartLoad)
            .parent().hide();
    } else {
        onCartLoad(cartContainer.data("got-cart") === false);
    }

    $(document)
        .on("click", ".shoppingcart .delete", function() {
            $(this).trigger("nwazet.removefromcart");
            setQuantityToZero("tr,li")($(this)).submit();
        })
        .on("click", ".minicart .delete", function() {
            $(this).trigger("nwazet.removefromcart");
            return cartContainerLoad(setQuantityToZero("tr,li")($(this)));
        })
        .on("click", ".minicart .update-button", function() {
            return cartContainerLoad($(this).closest("form"));
        })
        .on("submit", "form.addtocart", function(e) {
            $(this).trigger("nwazet.addtocart");
            e.preventDefault();
            var addForm = $(this),
                addFormData = addForm.serializeArray(),
                minicartForm = cartContainer.find("form"),
                productId = addForm.data("productid"),
                quantity = addForm.find("input[name=\"quantity\"]").val(),
                inputTag = "<input type=\"hidden\"/>",
                maxIndex = findNextIndex(minicartForm),
                attrIndex = 0,
                prefix = "items[" + maxIndex + "].";
            if (minicartForm.length !== 0) {
                minicartForm
                    .append($(inputTag).attr({
                        name: prefix + "ProductId",
                        value: productId
                    }))
                    .append($(inputTag).attr({
                        name: prefix + "Quantity",
                        value: quantity
                    }));
                var key, value, extra, attributePrefix;
                $.each(addFormData, function() {
                    var name = this.name;
                    if (name.substr(0, 19) === "productattributes.a") {
                        key = name.substr(19),
                            value = this.value,
                            extra = "testing",
                            attributePrefix = prefix + "AttributeIdsToValues[" + attrIndex++ + "].";
                    }
                    if (name.substr(0, 25) === "ext.productattributes.a") {
                        extra = this.value;
                    }
                });
                if (key && value) {
                    minicartForm
                        .append($(inputTag).attr({
                            name: attributePrefix + "Key",
                            value: key
                        }))
                        .append($(inputTag).attr({
                            name: attributePrefix + "Value.Value",
                            value: value
                        }))
                        .append($(inputTag).attr({
                            name: attributePrefix + "Value.ExtendedValue",
                            value: extra
                        }));
                }
                cartContainerLoad(minicartForm);
            } else {
                cartContainerLoad(addForm);
            }
            $(this).trigger("nwazet.addedtocart");
        });
});