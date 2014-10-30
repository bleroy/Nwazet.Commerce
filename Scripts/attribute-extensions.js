$(function () {
    var setExtensionVisibility = function (attr) {
        var attribute = $(attr),
            selectedOption = attribute.find("option:selected"),
            extensionId = attribute.attr("id");
        extensionName = selectedOption.data("extension-name"),
        extensions = attribute.parent().find("div.attribute-extension." + extensionId),
        hasExtension = selectedOption.data("has-extension");
        extensions.hide()
            .find("input,select").attr("disabled", "disabled");
        if (hasExtension) {
            attribute.parent().find("div.attribute-extension." + extensionName).show()
                .find("input,select").removeAttr("disabled");
        }
    };

    // Initialize visibility of attribute extensions
    $(".product-attribute").each(function (index, element) {
        setExtensionVisibility(element);
    })
    // Show / hide attribute extensions when selection changes
    .change(function () {
        setExtensionVisibility(this);
    });
});