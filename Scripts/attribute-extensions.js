$(function () {
    var setExtensionVisibility = function (attr) {
        var $attr = $(attr),
            $option = $attr.find("option:selected"),
            extensionId = $attr.attr("id");
        extensionName = $option.data("extension-name"),
        $extensions = $attr.parent().find("div.attribute-extension." + extensionId),
        hasExtension = $option.data("has-extension");
        $extensions.hide()
            .find("input,select").attr("disabled", "disabled");
        if (hasExtension) {
            $attr.parent().find("div.attribute-extension." + extensionName).show()
                .find("input,select").removeAttr("disabled");
        }
    };

    // Initialize visibility of attribute extensions
    $(".product-attribute").each(function (index, element) {
        setExtensionVisibility(element);
    });

    // Show / hide attribute extensions when selection changes
    $(".product-attribute").change(function () {
        setExtensionVisibility(this);
    });
});