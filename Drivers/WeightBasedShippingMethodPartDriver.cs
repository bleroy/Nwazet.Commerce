using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Shipping")]
    public class WeightBasedShippingMethodPartDriver : ContentPartDriver<WeightBasedShippingMethodPart> {
        private readonly IEnumerable<IShippingAreaProvider> _shippingAreaProviders;

        public WeightBasedShippingMethodPartDriver(IEnumerable<IShippingAreaProvider> shippingAreaProviders) {
            _shippingAreaProviders = shippingAreaProviders;
        }

        protected override string Prefix { get { return "NwazetCommerceWeightShipping"; } }

        protected override DriverResult Display(
            WeightBasedShippingMethodPart part, string displayType, dynamic shapeHelper) {
            return ContentShape(
                    "Parts_WeightBasedShippingMethod",
                    () => shapeHelper.Parts_WeightBasedShippingMethod(
                        Name: part.Name,
                        Price: part.Price,
                        ShippingCompany: part.ShippingCompany,
                        MinimumWeight: part.MinimumWeight,
                        MaximumWeight: part.MaximumWeight,
                        IncludedShippingAreas: part.IncludedShippingAreas.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries),
                        ExcludedShippingAreas: part.ExcludedShippingAreas.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries),
                        ContentItem: part.ContentItem));
        }

        //GET
        protected override DriverResult Editor(WeightBasedShippingMethodPart part, dynamic shapeHelper) {
            return ContentShape("Parts_WeightBasedShippingMethod_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/WeightBasedShippingMethod",
                    Model: shapeHelper.WeightShippingEditor(
                        ShippingMethod: part,
                        ShippingAreas: _shippingAreaProviders.SelectMany(ap => ap.GetAreas()),
                        Prefix: Prefix),
                    Prefix: Prefix));
        }

        private class LocalViewModel {
            public string[] IncludedShippingAreas { get; set; }
            public string[] ExcludedShippingAreas { get; set; }
        }

        //POST
        protected override DriverResult Editor(WeightBasedShippingMethodPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, new[] { "IncludedShippingAreas", "ExcludedShippingAreas" });
            var dyn = new LocalViewModel();
            updater.TryUpdateModel(dyn, Prefix, new[] { "IncludedShippingAreas", "ExcludedShippingAreas" }, null);
            part.IncludedShippingAreas = dyn.IncludedShippingAreas == null ?
                "" : string.Join(",", dyn.IncludedShippingAreas);
            part.ExcludedShippingAreas = dyn.ExcludedShippingAreas == null ? 
                "" : string.Join(",", dyn.ExcludedShippingAreas);
            return Editor(part, shapeHelper);
        }

        protected override void Importing(WeightBasedShippingMethodPart part, ImportContentContext context) {
            var name = context.Attribute(part.PartDefinition.Name, "Name");
            if (!String.IsNullOrWhiteSpace(name)) {
                part.Name = name;
            }
            var priceString = context.Attribute(part.PartDefinition.Name, "Price");
            double price;
            if (Double.TryParse(priceString, NumberStyles.Currency, CultureInfo.InvariantCulture, out price)) {
                part.Price = price;
            }
            var shippingCompany = context.Attribute(part.PartDefinition.Name, "ShippingCompany");
            if (!String.IsNullOrWhiteSpace(shippingCompany)) {
                part.ShippingCompany = shippingCompany;
            }
            var minimumWeight = context.Attribute(part.PartDefinition.Name, "MinimumWeight");
            double weight;
            if (minimumWeight != null && double.TryParse(minimumWeight, NumberStyles.Float, CultureInfo.InvariantCulture, out weight)) {
                part.MinimumWeight = weight;
            }
            var maximumWeight = context.Attribute(part.PartDefinition.Name, "MaximumWeight");
            if (maximumWeight != null && double.TryParse(maximumWeight, NumberStyles.Float, CultureInfo.InvariantCulture, out weight)) {
                part.MaximumWeight = weight;
            }
            var includedShippingAreas = context.Attribute(part.PartDefinition.Name, "IncludedShippingAreas");
            if (!String.IsNullOrWhiteSpace(includedShippingAreas)) {
                part.IncludedShippingAreas = includedShippingAreas;
            }
            var excludedShippingAreas = context.Attribute(part.PartDefinition.Name, "ExcludedShippingAreas");
            if (!String.IsNullOrWhiteSpace(excludedShippingAreas)) {
                part.ExcludedShippingAreas = excludedShippingAreas;
            }
        }

        protected override void Exporting(WeightBasedShippingMethodPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Name", part.Name);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Price", part.Price.ToString("C"));
            context.Element(part.PartDefinition.Name).SetAttributeValue("ShippingCompany", part.ShippingCompany);
            if (part.MinimumWeight != null && !double.IsNaN((double)part.MinimumWeight)) {
                context.Element(part.PartDefinition.Name).SetAttributeValue(
                    "MinimumWeight", ((double)part.MinimumWeight).ToString(CultureInfo.InvariantCulture));
            }
            if (part.MaximumWeight != null && !double.IsNaN((double)part.MaximumWeight)) {
                context.Element(part.PartDefinition.Name).SetAttributeValue(
                    "MaximumWeight", ((double)part.MaximumWeight).ToString(CultureInfo.InvariantCulture));
            }
            context.Element(part.PartDefinition.Name).SetAttributeValue("IncludedShippingAreas", part.IncludedShippingAreas);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ExcludedShippingAreas", part.IncludedShippingAreas);
        }
    }
}
