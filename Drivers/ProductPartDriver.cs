using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nwazet.Commerce.Helpers;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductPartDriver : ContentPartDriver<ProductPart> {
        private readonly IWorkContextAccessor _wca;
        private readonly IPriceService _priceService;
        private readonly IEnumerable<IProductAttributesDriver> _attributeProviders;

        public ProductPartDriver(
            IWorkContextAccessor wca,
            IPriceService priceService,
            IEnumerable<IProductAttributesDriver> attributeProviders) {

            _wca = wca;
            _priceService = priceService;
            _attributeProviders = attributeProviders;
        }

        protected override string Prefix {
            get { return "NwazetCommerceProduct"; }
        }

        protected override DriverResult Display(
            ProductPart part, string displayType, dynamic shapeHelper) {

            var inventory = GetInventory(part);
            var discountedPriceQuantity = _priceService.GetDiscountedPrice(new ShoppingCartQuantityProduct(1, part));
            var productShape = ContentShape(
                "Parts_Product",
                () => shapeHelper.Parts_Product(
                    Sku: part.Sku,
                    Price: part.Price,
                    DiscountedPrice: discountedPriceQuantity.Price,
                    DiscountComment: discountedPriceQuantity.Comment,
                    Inventory: inventory,
                    OutOfStockMessage: part.OutOfStockMessage,
                    AllowBackOrder: part.AllowBackOrder,
                    Weight: part.Weight,
                    Size: part.Size,
                    ShippingCost: part.ShippingCost,
                    IsDigital: part.IsDigital,
                    ContentPart: part
                    )
                );
            if (part.Inventory > 0 || part.AllowBackOrder) {
                return Combined(
                    productShape,
                    ContentShape(
                        "Parts_Product_AddButton",
                        () => {
                            // Get attributes and add them to the add to cart shape
                            var attributeShapes = _attributeProviders
                                .Select(p => p.GetAttributeDisplayShape(part.ContentItem, shapeHelper))
                                .ToList();
                            return shapeHelper.Parts_Product_AddButton(
                                ProductId: part.Id,
                                ProductAttributes: attributeShapes);
                        })
                    );
            }
            return productShape;
        }

        private int GetInventory(ProductPart part) {
            IBundleService bundleService;
            var inventory = part.Inventory;
            if (_wca.GetContext().TryResolve(out bundleService) && part.Has<BundlePart>()) {
                var bundlePart = part.As<BundlePart>();
                var ids = bundlePart.ProductIds.ToList();
                if (!ids.Any()) return 0;
                part.Inventory = inventory =
                    bundleService
                        .GetProductQuantitiesFor(bundlePart)
                        .Min(p => p.Product.Inventory/p.Quantity);
            }
            return inventory;
        }

        //GET
        protected override DriverResult Editor(ProductPart part, dynamic shapeHelper) {
            part.Weight = Math.Round(part.Weight, 3);
            part.Inventory = GetInventory(part);
            return ContentShape("Parts_Product_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Product",
                    Model: part,
                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(
            ProductPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Importing(ProductPart part, ImportContentContext context) {
            var el = context.Data.Element(typeof (ProductPart).Name);
            if (el == null) return;
            el.With(part)
                .FromAttr(p => p.Sku)
                .FromAttr(p => p.Inventory)
                .FromAttr(p => p.OutOfStockMessage)
                .FromAttr(p => p.AllowBackOrder)
                .FromAttr(p => p.IsDigital)
                .FromAttr(p => p.Weight);
            var priceAttr = el.Attribute("Price");
            double price;
            if (priceAttr != null &&
                double.TryParse(priceAttr.Value, NumberStyles.Currency, CultureInfo.InvariantCulture, out price)) {
                part.Price = price;
            }
            var shippingCostAttr = el.Attribute("ShippingCost");
            double shippingCost;
            if (shippingCostAttr != null &&
                double.TryParse(shippingCostAttr.Value, NumberStyles.Currency, CultureInfo.InvariantCulture,
                    out shippingCost)) {
                part.ShippingCost = shippingCost;
            }
        }

        protected override void Exporting(ProductPart part, ExportContentContext context) {
            var el = context.Element(typeof (ProductPart).Name);
            el
                .With(part)
                .ToAttr(p => p.Sku)
                .ToAttr(p => p.Inventory)
                .ToAttr(p => p.OutOfStockMessage)
                .ToAttr(p => p.AllowBackOrder)
                .ToAttr(p => p.IsDigital)
                .ToAttr(p => p.Weight);
            el.SetAttributeValue("Price", part.Price.ToString("C", CultureInfo.InvariantCulture));
            if (part.ShippingCost != null) {
                el.SetAttributeValue(
                    "ShippingCost", ((double) part.ShippingCost).ToString("C", CultureInfo.InvariantCulture));
            }
        }
    }
}
