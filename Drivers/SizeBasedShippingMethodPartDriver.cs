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

        protected override string Prefix {
            get { return "NwazetCommerceWeightShipping"; }
        }

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
                    IncludedShippingAreas:
                        part.IncludedShippingAreas.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries),
                    ExcludedShippingAreas:
                        part.ExcludedShippingAreas.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries),
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
        protected override DriverResult Editor(SizeBasedShippingMethodPart part, IUpdateModel updater,
            dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, new[] {"IncludedShippingAreas", "ExcludedShippingAreas"});
            var dyn = new LocalViewModel();
            updater.TryUpdateModel(dyn, Prefix, new[] {"IncludedShippingAreas", "ExcludedShippingAreas"}, null);
            part.IncludedShippingAreas = dyn.IncludedShippingAreas == null
                ? ""
                : string.Join(",", dyn.IncludedShippingAreas);
            part.ExcludedShippingAreas = dyn.ExcludedShippingAreas == null
                ? ""
                : string.Join(",", dyn.ExcludedShippingAreas);
            return Editor(part, shapeHelper);
        }

        protected override void Importing(SizeBasedShippingMethodPart part, ImportContentContext context) {
            var el = context.Data.Element(typeof (SizeBasedShippingMethodPart).Name);
            if (el == null) return;
            el.With(part)
                .FromAttr(p => p.Name)
                .FromAttr(p => p.ShippingCompany)
                .FromAttr(p => p.Size)
                .FromAttr(p => p.Priority)
                .FromAttr(p => p.IncludedShippingAreas)
                .FromAttr(p => p.ExcludedShippingAreas);
            var priceAttr = el.Attribute("Price");
            decimal price;
            if (priceAttr != null &&
                decimal.TryParse(priceAttr.Value, NumberStyles.Currency, CultureInfo.InvariantCulture, out price)) {
                part.Price = price;
            }
        }

        protected override void Exporting(SizeBasedShippingMethodPart part, ExportContentContext context) {
            var el = context.Element(typeof (SizeBasedShippingMethodPart).Name);
            el.With(part)
                .ToAttr(p => p.Name)
                .ToAttr(p => p.ShippingCompany)
                .ToAttr(p => p.Size)
                .ToAttr(p => p.Priority)
                .ToAttr(p => p.IncludedShippingAreas)
                .ToAttr(p => p.ExcludedShippingAreas);
            el.SetAttributeValue("Price", part.Price.ToString("C"));
        }
    }
}
