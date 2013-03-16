using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributesPartDriver : ContentPartDriver<ProductAttributesPart>, IProductAttributesDriver {
        private readonly IContentManager _contentManager;

        public ProductAttributesPartDriver(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        protected override string Prefix { get { return "NwazetCommerceAttribute"; } }

        protected override DriverResult Display(
            ProductAttributesPart part, string displayType, dynamic shapeHelper) {

            // The shape is acquired by the product driver so it can be included into the add to cart shape
            return null;
        }

        public dynamic GetAttributeDisplayShape(IContent product, dynamic shapeHelper) {
            var attributesPart = product.As<ProductAttributesPart>();
            var attributes = attributesPart == null ? null : _contentManager
                .GetMany<ProductAttributePart>(
                    attributesPart.AttributeIds,
                    VersionOptions.Published,
                    new QueryHints().ExpandParts<TitlePart>());
            return shapeHelper.Parts_ProductAttributes(
                ContentItem: product,
                ProductAttributes: attributes
                );
        }

        public bool ValidateAttributes(IContent product, IDictionary<int, string> attributeIdsToValues) {
            var attributesPart = product.As<ProductAttributesPart>();
            //if (attributeIdsToValues.Count == 1 &&
            //    attributeIdsToValues[0].)
            // If the part isn't there, there must be no attributes
            if (attributesPart == null) return attributeIdsToValues == null || !attributeIdsToValues.Any();
            // If the part is there, it must have as many attributes as were passed in
            if (attributesPart.AttributeIds.Count() != attributeIdsToValues.Count) return false;
            // The same attributes must be present
            if (!attributesPart.AttributeIds.All(attributeIdsToValues.ContainsKey)) return false;
            // Get the actual attributes in order to verify the values
            var attributes = _contentManager.GetMany<ProductAttributePart>(
                attributeIdsToValues.Keys, 
                VersionOptions.Published, 
                QueryHints.Empty)
                .ToList();
            // The values that got passed in must exist
            return attributes.All(attribute => attribute.AttributeValues.Contains(attributeIdsToValues[attribute.Id]));
        }

        //GET
        protected override DriverResult Editor(ProductAttributesPart part, dynamic shapeHelper) {
            return ContentShape(
                "Parts_ProductAttributes_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/ProductAttributes",
                    Prefix: Prefix,
                    Model: new ProductAttributesPartEditViewModel {
                        Prefix = Prefix,
                        Part = part,
                        Attributes = _contentManager
                        .Query<ProductAttributePart>(VersionOptions.Published)
                        .Join<TitlePartRecord>()
                        .OrderBy(p => p.Title)
                        .List()
                    }));
        }

        //POST
        protected override DriverResult Editor(ProductAttributesPart part, IUpdateModel updater, dynamic shapeHelper) {
            var editViewModel = new ProductAttributesEditViewModel();
            if (updater.TryUpdateModel(editViewModel, Prefix, null, null)) {
                part.AttributeIds = editViewModel.AttributeIds;
            }
            return Editor(part, shapeHelper);
        }

        private class ProductAttributesEditViewModel {
            public int[] AttributeIds { get; [UsedImplicitly] set; }
        }

        protected override void Importing(ProductAttributesPart part, ImportContentContext context) {
            var values = context.Attribute(part.PartDefinition.Name, "Ids");
            if (!String.IsNullOrWhiteSpace(values)) {
                part.Record.Attributes = values;
            }
        }

        protected override void Exporting(ProductAttributesPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Ids", part.Record.Attributes);
        }
    }
}
