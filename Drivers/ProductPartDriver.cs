using System;
using System.Globalization;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Commerce")]
    public class ProductPartDriver : ContentPartDriver<ProductPart> {
        protected override string Prefix { get { return "NwazetCommerceProduct"; } }

        protected override DriverResult Display(
            ProductPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                 ContentShape("Parts_Product",
                    () => shapeHelper.Parts_Product(
                        Sku: part.Sku,
                        Price: part.Price,
                        Weight: part.Weight,
                        ShippingCost: part.ShippingCost,
                        IsDigital: part.IsDigital
                        )),
                 ContentShape("Parts_Product_AddButton", () => shapeHelper.Parts_Product_AddButton(
                     ProductId: part.Id
                     ))
                );
        }

        //GET
        protected override DriverResult Editor(ProductPart part, dynamic shapeHelper) {
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
            context.Element(part.PartDefinition.Name).SetAttributeValue("IsDigital", part.IsDigital.ToString(CultureInfo.InvariantCulture).ToLower());
            context.Element(part.PartDefinition.Name).SetAttributeValue("Weight", part.Weight.ToString(CultureInfo.InvariantCulture));
            if (part.ShippingCost != null) {
                context.Element(part.PartDefinition.Name).SetAttributeValue(
                    "ShippingCost", ((double)part.ShippingCost).ToString("C"));
            }
        }
    }
}
