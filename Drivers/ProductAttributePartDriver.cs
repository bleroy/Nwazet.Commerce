using System;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributePartDriver : ContentPartDriver<ProductAttributePart> {

        private readonly IEnumerable<IProductAttributeExtensionProvider> _attributeExtensionProviders;

        public ProductAttributePartDriver(
           IOrchardServices services,
           IEnumerable<IProductAttributeExtensionProvider> attributeExtensionProviders) {

            Services = services;
            _attributeExtensionProviders = attributeExtensionProviders;
        }

        public IOrchardServices Services { get; set; }

        protected override string Prefix { get { return "NwazetCommerceAttribute"; } }

        protected override DriverResult Display(
            ProductAttributePart part, string displayType, dynamic shapeHelper) {
            // The attribute part should never appear on the front-end.
            return null;
        }

        //GET
        protected override DriverResult Editor(ProductAttributePart part, dynamic shapeHelper) {
            return ContentShape(
                "Parts_ProductAttribute_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/ProductAttribute",
                    Prefix: Prefix,
                    Model: new ProductAttributePartEditViewModel {
                        DisplayName = part.DisplayName,
                        TechnicalName = part.TechnicalName,
                        SortOrder = part.SortOrder,
                        AttributeValues = part.AttributeValues,
                        AttributeExtensionProviders = _attributeExtensionProviders
                    }));
        }

        //POST
        protected override DriverResult Editor(ProductAttributePart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Importing(ProductAttributePart part, ImportContentContext context) {
            var values = context.Attribute(part.PartDefinition.Name, "Values");
            if (!String.IsNullOrWhiteSpace(values)) {
                part.Record.AttributeValues = values;
            }
        }

        protected override void Exporting(ProductAttributePart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Values", part.Record.AttributeValues);
        }
    }
}
