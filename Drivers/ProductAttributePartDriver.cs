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
using Orchard.Localization;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributePartDriver : ContentPartDriver<ProductAttributePart> {

        private readonly IEnumerable<IProductAttributeExtensionProvider> _attributeExtensionProviders;
        private readonly IProductAttributeNameService _productAttributeNameService;

        public ProductAttributePartDriver(
           IOrchardServices services,
           IEnumerable<IProductAttributeExtensionProvider> attributeExtensionProviders,
            IProductAttributeNameService productAttributeNameService) {

            Services = services;
            _attributeExtensionProviders = attributeExtensionProviders;
            _productAttributeNameService = productAttributeNameService;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
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
            if (updater.TryUpdateModel(part, Prefix, null, null)) {
                //check TechnicalName for invalid characters
                if (!String.Equals(part.TechnicalName, part.TechnicalName.ToSafeName(), StringComparison.OrdinalIgnoreCase)) {
                    updater.AddModelError("Name", T("The technical name contains invalid characters."));
                }
                //ensure uniqueness of TechnicalName
                var tName = part.TechnicalName;
                if (!_productAttributeNameService.ProcessTechnicalName(part)) {
                    Services.Notifier.Warning(
                        T("Attribute technical names in conflict. \"{0}\" is already set for a previously created attribute so now it has been changed to \"{1}\"",
                        tName, part.TechnicalName));
                }
            }
            return Editor(part, shapeHelper);
        }

        protected override void Importing(ProductAttributePart part, ImportContentContext context) {
            var values = context.Attribute(part.PartDefinition.Name, "Values");
            if (!String.IsNullOrWhiteSpace(values)) {
                part.Record.AttributeValues = values;
            }
            part.DisplayName = context.Attribute(part.PartDefinition.Name, "DisplayName");
            part.TechnicalName = context.Attribute(part.PartDefinition.Name, "TechnicalName");
            int so = 0;
            int.TryParse(context.Attribute(part.PartDefinition.Name, "SortOrder"), out so);
            part.SortOrder = so;
        }

        protected override void Exporting(ProductAttributePart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("SortOrder", part.SortOrder);
            context.Element(part.PartDefinition.Name).SetAttributeValue("DisplayName", part.DisplayName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("TechnicalName", part.TechnicalName);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Values", part.Record.AttributeValues);
        }
    }
}
