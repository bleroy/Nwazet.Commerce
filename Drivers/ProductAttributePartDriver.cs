using System;
using JetBrains.Annotations;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using System.Collections;
using System.Collections.Generic;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributePartDriver : ContentPartDriver<ProductAttributePart> {

        public ProductAttributePartDriver(
            IOrchardServices services) {

            Services = services;
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
                    Model: part));
        }

        //POST
        protected override DriverResult Editor(ProductAttributePart part, IUpdateModel updater, dynamic shapeHelper) {
            var editViewModel = new ProductAttributeEditViewModel();
            if (updater.TryUpdateModel(editViewModel, Prefix, null, null)) {
                part.AttributeValues = editViewModel.AttributeValues;
            }
            return Editor(part, shapeHelper);
        }

        private class ProductAttributeEditViewModel {
            public ICollection<ProductAttributeValue> AttributeValues { get; set; }
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
