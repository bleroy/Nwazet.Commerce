using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Tokens;

namespace Nwazet.Commerce.Tokens {
    [OrchardFeature("Nwazet.Commerce")]
    public class CartTokens : ITokenProvider {
        private readonly IShoppingCart _shoppingCart;

        public CartTokens(IShoppingCart shoppingCart) {
            T = NullLocalizer.Instance;
            _shoppingCart = shoppingCart;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeContext context) {
            context.For("Cart", T("Shopping Cart"), T("Shopping Cart"))
                .Token("Items", T("Items"), T("The items in the shopping cart."), "CartItems")
                .Token("Country", T("Country"), T("The country where the cart will get shipped."))
                .Token("ZipCode", T("Zip Code"), T("The zip code where the cart will get shipped."))
                .Token("Shipping", T("Shipping Option"), T("The shipping option chosen by the user."), "ShippingOption")
                .Token("Subtotal", T("Subtotal"), T("The total price of all products in the cart, without the tax and shipping costs."))
                .Token("Taxes", T("Taxes"), T("The taxes to be paid on this cart."), "TaxAmount")
                .Token("Total", T("Total"), T("The total price of all products in the cart, with the tax and shipping costs included."))
                .Token("Count", T("Item count"), T("The total number of products in the cart."));

            context.For("CartItems", T("Cart Items"), T("The list of product quantities and prices in the cart."))
                .Token("Format:*", T("Format:<separator>:<cart format>"), T("Formats the contents of the cart using a format string that uses $quantity for the quantity, $product for the product name (with attributes), and $price for the price. For example {Cart.Items.Format:, :$quantity x $product}"), "Text");

            context.For("ShippingOption", T("Shipping Option"), T("Shipping Option"))
                .Token("Price", T("Price"), T("Price of the shipping option"))
                .Token("Description", T("Description"), T("Description of the shipping option"), "Text")
                .Token("Company", T("Company"), T("The shipping company"), "Text");

            context.For("TaxAmount", T("Tax Amount"), T("Tax Amount"))
                .Token("Name", T("Name"), T("Name of the tax"), "Text")
                .Token("Amount", T("Amount"), T("Amount taxed"));
        }

        public void Evaluate(EvaluateContext context) {
            context.For<IShoppingCart>("Cart")
                .Token("Items", c => string.Join(T(", ").Text,
                    (c ?? _shoppingCart).GetProducts().Select(p => p.ToString())))
                .Chain("Items", "CartItems", c => (c ?? _shoppingCart).GetProducts())
                .Token("Country", c => (c ?? _shoppingCart).Country)
                .Token("ZipCode", c => (c ?? _shoppingCart).ZipCode)
                .Token("Shipping", c => (c ?? _shoppingCart).ShippingOption.ToString())
                .Chain("Shipping", "ShippingOption", c => (c ?? _shoppingCart).ShippingOption)
                .Token("Subtotal", c => (c ?? _shoppingCart).Subtotal().ToString("C"))
                .Token("Taxes", c => {
                    var taxes = (c ?? _shoppingCart).Taxes();
                    return taxes == null ? 0.ToString("C") : taxes.Amount.ToString("C");
                })
                .Chain("Taxes", "TaxAmount", c => (c ?? _shoppingCart).Taxes() ?? new TaxAmount {Amount = 0, Name = ""})
                .Token("Total", c => (c ?? _shoppingCart).Total().ToString("C"))
                .Token("Count", c => (c ?? _shoppingCart).ItemCount());

            context.For<IEnumerable<ShoppingCartQuantityProduct>>("CartItems")
                .Token(s => s.StartsWith("Format:", StringComparison.OrdinalIgnoreCase) ? s.Substring("Format:".Length) : null,
                    (s, q) => {
                        var separator = "";
                        var colonIndex = s.IndexOf(':');
                        if (colonIndex != -1 && s.Length > colonIndex + 1) {
                            separator = s.Substring(0, colonIndex);
                            s = s.Substring(colonIndex + 1);
                        }
                        var format = s
                            .Replace("$quantity", "{0}")
                            .Replace("$sku", "{1}")
                            .Replace("$product", "{2}")
                            .Replace("$price", "{3}");
                        return String.Join(separator, q.Select(qp =>
                            String.Format(format,
                                qp.Quantity,
                                qp.Product.Sku,
                                qp.Product.As<TitlePart>().Title + " " + qp.AttributeDescription,
                                qp.Price.ToString("C"))));
                    });

            context.For<ShippingOption>("ShippingOption")
                .Token("Price", option => option.Price.ToString("C"))
                .Token("Description", option => option.Description)
                .Chain("Description", "Text", option => option.Description)
                .Token("Company", option => option.ShippingCompany)
                .Chain("Company", "Text", option => option.ShippingCompany);

            context.For<TaxAmount>("TaxAmount")
                .Token("Name", amount => amount.Name)
                .Chain("Name", "Text", amount => amount.Name)
                .Token("Amount", amount => amount.Amount.ToString("C"));
        }
    }
}