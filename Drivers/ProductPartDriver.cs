using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public ProductPartDriver(IWorkContextAccessor wca) {
            _wca = wca;
        }

        protected override string Prefix { get { return "NwazetCommerceProduct"; } }

        protected override DriverResult Display(
            ProductPart part, string displayType, dynamic shapeHelper) {

            var inventory = GetInventory(part);
            var productShape = ContentShape(
                "Parts_Product",
                () => shapeHelper.Parts_Product(
                        Sku: part.Sku,
                        Price: part.Price,
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
                        () => shapeHelper.Parts_Product_AddButton(ProductId: part.Id))
                    );
            }
            return productShape;
        }

        private int GetInventory(ProductPart part) {
            IBundleService bundleService;
            var inventory = part.Inventory;
            if (_wca.GetContext().TryResolve(out bundleService) && part.Has<BundlePart>()) {
                var bundlePart = part.As<BundlePart>();
                if (!bundlePart.ProductIds.Any()) return 0;
                part.Inventory = inventory =
                    bundleService
                    .GetProductQuantitiesFor(bundlePart)
                    .Min(p => p.Product.Inventory / p.Quantity);
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
            var sku = context.Attribute(part.PartDefinition.Name, "Sku");
            if (!String.IsNullOrWhiteSpace(sku)) {
                part.Sku = sku;
            }
            var priceString = context.Attribute(part.PartDefinition.Name, "Price");
            double price;
            if (double.TryParse(priceString, NumberStyles.Currency, CultureInfo.InvariantCulture, out price)) {
                part.Price = price;
            }
            var inventoryString = context.Attribute(part.PartDefinition.Name, "Inventory");
            int inventory;
            if (int.TryParse(inventoryString, out inventory)) {
                part.Inventory = inventory;
            }
            var outOfStockMessage = context.Attribute(part.PartDefinition.Name, "OutOfStockMessage");
            if (!String.IsNullOrWhiteSpace(outOfStockMessage)) {
                part.OutOfStockMessage = outOfStockMessage;
            }
            var allowBackOrderString = context.Attribute(part.PartDefinition.Name, "AllowBackOrder");
            bool allowBackOrder;
            if (bool.TryParse(allowBackOrderString, out allowBackOrder)) {
                part.AllowBackOrder = allowBackOrder;
            }
            var isDigitalAttribute = context.Attribute(part.PartDefinition.Name, "IsDigital");
            bool isDigital;
            if (bool.TryParse(isDigitalAttribute, out isDigital)) {
                part.IsDigital = isDigital;
            }
            var weightString = context.Attribute(part.PartDefinition.Name, "Weight");
            double weight;
            if (double.TryParse(weightString, NumberStyles.Float, CultureInfo.InvariantCulture, out weight)) {
                part.Weight = weight;
            }
            var shippingCostString = context.Attribute(part.PartDefinition.Name, "ShippingCost");
            double shippingCost;
            if (shippingCostString != null && double.TryParse(shippingCostString, NumberStyles.Currency, CultureInfo.InvariantCulture, out shippingCost)) {
                part.ShippingCost = shippingCost;
            }
        }

        protected override void Exporting(ProductPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Sku", part.Sku);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Price", part.Price.ToString("C"));
            context.Element(part.PartDefinition.Name).SetAttributeValue("Inventory", part.Inventory.ToString(CultureInfo.InvariantCulture));
            context.Element(part.PartDefinition.Name).SetAttributeValue("OutOfStockMessage", part.OutOfStockMessage);
            context.Element(part.PartDefinition.Name).SetAttributeValue("AllowBackOrder", part.AllowBackOrder.ToString(CultureInfo.InvariantCulture).ToLower());
            context.Element(part.PartDefinition.Name).SetAttributeValue("IsDigital", part.IsDigital.ToString(CultureInfo.InvariantCulture).ToLower());
            context.Element(part.PartDefinition.Name).SetAttributeValue("Weight", part.Weight.ToString(CultureInfo.InvariantCulture));
            if (part.ShippingCost != null) {
                context.Element(part.PartDefinition.Name).SetAttributeValue(
                    "ShippingCost", ((double)part.ShippingCost).ToString("C"));
            }
        }
    }
}
