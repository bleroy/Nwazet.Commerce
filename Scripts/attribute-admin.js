$(function () {

    var i = $("#NwazetCommerceAttribute_AttributeValues > li.option").length; // indexer for mvc model binding new tiers

    $("#NwazetCommerceAttribute_AddAttributeValue").click(function (event) {
        event.preventDefault();
        var valueTemplate = $("#valueTemplate").html();
        $("#NwazetCommerceAttribute_AttributeValues").append(
            Mustache.render(valueTemplate, { index: i, sort: i + 1 }));
        i++;
    });

    $("#NwazetCommerceAttribute_AttributeValues").on("click", ".nwazet-remove-attribute-value", function (event) {
        event.preventDefault();
        $(this).parents("li").remove();
    });

    $("#NwazetCommerceAttribute_AttributeValues").sortable({
        update: function (event, ui) {
            $.each($(this).children("li"), function () {
                var $row = $(this);
                $row.find("input[name$=SortOrder]").val($row.index());
            });
        }
    });
});