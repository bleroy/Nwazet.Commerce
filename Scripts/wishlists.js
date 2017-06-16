jQuery(function ($) {
    $('#wishlist-add').click(function () {
        var cartAttributes = $('#add-to-cart-attributes'); //attributes from "add to cart" button
        cartAttributes.clone().appendTo('#add-to-wishlist-attributes');
        //jQuery does not clone selected values: https://stackoverflow.com/a/743871/2669614
        var wishListAttributes = $('#add-to-wishlist-attributes');
        var selects = cartAttributes.find("select");
        $(selects).each(function (i) {
            var select = this;
            wishListAttributes.find("select").eq(i).val($(select).val());
        });
    });
});