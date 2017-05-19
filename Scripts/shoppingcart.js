jQuery(function($) {
    var hasLocalStorage = function() {
            try {
                return "localStorage" in window && window.localStorage !== null;
            } catch(e) {
                return false;
            }
        },
        setUseLocalStorage = function (data, status) {
            useLocalStorage = !!(data.Response);
        },
        useLocalStorage = true,
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
                    if (useLocalStorage) {
                        if (gotCart) {
                            var form = cartContainer.find("form");
                            if (form.length === 0) {
                                form = cartContainer.closest("form");
                            }
                            if (form.length !== 0 && form[0].length > 1) {
                                var cartArray = form.serializeArray(),
                                    cart = {};
                                $.each(cartArray, function (index, formField) {
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
                                } catch (ex) {
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
                    } else {
                        localStorage.removeItem("nwazet.cart");
                        localStorage.removeItem("nwazet-cart-loading");
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
        token = cartContainer.data("token"),
        useLocalStoragePath = cartContainer.data("use-local-storage");

    if (useLocalStoragePath) {
        var formData = {
            __RequestVerificationToken: token
        };
        $.ajax({
            url: useLocalStoragePath,
            data: formData,
            type: 'POST',
            async: false,
            success: setUseLocalStorage
        });
    }

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
                action = this.action,
                fileInputs = $("input[type=file]:enabled", addForm);
            if (fileInputs.length > 0) {
                // Flag as ajax request, controller can't detect this when using iframe
                addFormData.push({ name: "isAjaxRequest", value: true });
                $.ajax(action, {
                    type: "POST",
                    data: addFormData,
                    files: fileInputs,
                    iframe: true
                }).done(function (content) {
                    cartContainer.html(content);
                    onCartLoad(content, "success");
                });
            } else {
                cartContainer.load(action, addFormData, onCartLoad);
            }
            $(this).trigger("nwazet.addedtocart");
        });
});