$(function() {
    var i = $("#NwazetCommerceAttribute_AttributeValues > li.option").length; // indexer for mvc model binding new tiers

    $("#NwazetCommerceAttribute_AddAttributeValue").click(function (event) {
        event.preventDefault();
        $("#NwazetCommerceAttribute_AttributeValues").append('<li class="option">' +
                                                        '<div class="option-name"><input type="hidden" name="NwazetCommerceAttribute.AttributeValues.Index" value="' + i + '" />' +
                                                        '<input type="hidden" name="NwazetCommerceAttribute.AttributeValues[' + i + '].SortOrder" value="' + (i + 1) + '" />' +
                                                        '<input type="text" class="text" name="NwazetCommerceAttribute.AttributeValues[' + i + '].Text" value="" /></div>\r\n' +
                                                        '<div class="price-adj"><input type="text" class="text small" name="NwazetCommerceAttribute.AttributeValues[' + i + '].PriceAdjustment" value="0" /></div>\r\n' +
                                                        '<div class="apply-line"><input type="checkbox" name="NwazetCommerceAttribute.AttributeValues[' + i + '].IsLineAdjustment" value="True" /></div>\r\n' +
                                                        '<div><a href="#" class="nwazet-remove-attribute-value">Delete</a></div></li>');
        i++;
    });

    $("#NwazetCommerceAttribute_AttributeValues").on("click", ".nwazet-remove-attribute-value", function (event) {
        event.preventDefault();
        $(this).parents("li").remove();
    });

    $("#NwazetCommerceAttribute_AttributeValues").sortable({
        update: function(event, ui) {
            $.each($(this).children("li"), function() {
                var $row = $(this);
                $row.find("input[name$=SortOrder]").val($row.index());
            });
        }
    });
});