jQuery(function ($) {
    $(".inventory .remove").click(function () {
        $.post($(this).data("url"), {
                id: $(this).data("id"),
                __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val()
            }, function(data) {
                for (var sku in data) {
                    $(".inventory .value." + sku)
                        .data("inventory", data[sku])
                        .slideUp(function() {
                            var that = $(this);
                            that.html(that.data("inventory")).slideDown();
                        });
                }
            });
        return false;
    });
});
