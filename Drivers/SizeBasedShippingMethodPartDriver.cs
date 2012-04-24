using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Shipping")]
    public class SizeBasedShippingMethodPartDriver : ContentPartDriver<SizeBasedShippingMethodPart> {
        private readonly IEnumerable<IShippingAreaProvider> _shippingAreaProviders;

        public SizeBasedShippingMethodPartDriver(IEnumerable<IShippingAreaProvider> shippingAreaProviders) {
            _shippingAreaProviders = shippingAreaProviders;
        }

        protected override string Prefix { get { return "NwazetCommerceWeightShipping"; } }

        protected override DriverResult Display(
            SizeBasedShippingMethodPart part, string displayType, dynamic shapeHelper) {
            return ContentShape(
                    "Parts_SizeBasedShippingMethod",
                    () => shapeHelper.Parts_SizeBasedShippingMethod(
                        Name: part.Name,
                        Price: part.Price,
                        ShippingCompany: part.ShippingCompany,
                        Size: part.Size,
                        Priority: part.Priority,
                        IncludedShippingAreas: part.IncludedShippingAreas.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries),
                        ExcludedShippingAreas: part.ExcludedShippingAreas.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries),
                        ContentItem: part.ContentItem));
        }

        //GET
        protected override DriverResult Editor(SizeBasedShippingMethodPart part, dynamic shapeHelper) {
            return ContentShape("Parts_SizeBasedShippingMethod_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/SizeBasedShippingMethod",
                    Model: shapeHelper.SizeShippingEditor(
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
        protected override DriverResult Editor(SizeBasedShippingMethodPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, new[] { "IncludedShippingAreas", "ExcludedShippingAreas" });
            var dyn = new LocalViewModel();
            updater.TryUpdateModel(dyn, Prefix, new[] { "IncludedShippingAreas", "ExcludedShippingAreas" }, null);
            part.IncludedShippingAreas = dyn.IncludedShippingAreas == null ?
                "" : string.Join(",", dyn.IncludedShippingAreas);
            part.ExcludedShippingAreas = dyn.ExcludedShippingAreas == null ? 
                "" : string.Join(",", dyn.ExcludedShippingAreas);
            return Editor(part, shapeHelper);
        }

        protected override void Importing(SizeBasedShippingMethodPart part, ImportContentContext context) {
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
            var size = context.Attribute(part.PartDefinition.Name, "Size");
            if (!string.IsNullOrWhiteSpace(size)) {
                part.Size = size;
            }
            var priorityString = context.Attribute(part.PartDefinition.Name, "Priority");
            int priority;
            if (priorityString != null && int.TryParse(priorityString, NumberStyles.Integer, CultureInfo.InvariantCulture, out priority)) {
                part.Priority = priority;
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

        protected override void Exporting(SizeBasedShippingMethodPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Name", part.Name);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Price", part.Price.ToString("C"));
            context.Element(part.PartDefinition.Name).SetAttributeValue("ShippingCompany", part.ShippingCompany);
            if (!string.IsNullOrWhiteSpace(part.Size)) {
                context.Element(part.PartDefinition.Name).SetAttributeValue("Size", part.Size);
            }
            context.Element(part.PartDefinition.Name).SetAttributeValue("Priority", part.Priority.ToString(CultureInfo.InvariantCulture));
            context.Element(part.PartDefinition.Name).SetAttributeValue("IncludedShippingAreas", part.IncludedShippingAreas);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ExcludedShippingAreas", part.IncludedShippingAreas);
        }
    }
}
