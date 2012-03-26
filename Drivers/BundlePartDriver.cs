using System.Linq;
using System.Xml.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Bundles")]
    public class BundlePartDriver : ContentPartDriver<BundlePart> {
        private readonly IBundleService _bundleService;
        private readonly IContentManager _contentManager;

        public BundlePartDriver(IBundleService bundleService, IContentManager contentManager) {
            _bundleService = bundleService;
            _contentManager = contentManager;
        }

        protected override string Prefix {
            get { return "Bundle"; }
        }

        protected override DriverResult Display(BundlePart part, string displayType, dynamic shapeHelper) {
            var products = _contentManager.GetMany<ProductPart>(
                part.ProductIds, VersionOptions.Published, QueryHints.Empty);
            return ContentShape(
                "Parts_Bundle",
                () => shapeHelper.Parts_Bundle(
                    ContentPart: part,
                    Products: products.Select(
                    p => _contentManager.BuildDisplay(p, "Thumbnail"))));
        }

        protected override DriverResult Editor(BundlePart part, dynamic shapeHelper) {
            return ContentShape("Parts_Bundle_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Bundle",
                    Model: _bundleService.BuildEditorViewModel(part),
                    Prefix: Prefix));
        }

        protected override DriverResult Editor(BundlePart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new BundleViewModel();
            updater.TryUpdateModel(model, Prefix, null, null);

            if (part.ContentItem.Id != 0) {
                _bundleService.UpdateBundleProducts(part.ContentItem, model.Products);
            }
            return Editor(part, shapeHelper);
        }

        protected override void Importing(BundlePart part, ImportContentContext context) {
            var bundledProducts =
                context.Data.Element("BundlePart").Elements("Product")
                    .Select(e => context.GetItemFromSession(e.Attribute("id").Value));
            foreach (var bundledProduct in bundledProducts) {
                _bundleService.AddProduct(bundledProduct.Id, part.Record);
            }
        }

        protected override void Exporting(BundlePart part, ExportContentContext context) {
            var elt = context.Element("BundlePart");
            foreach (var productId in part.ProductIds) {
                var productElement = new XElement("Product");
                productElement.SetAttributeValue("id",
                    context.ContentManager.GetItemMetadata(
                    context.ContentManager.Get(productId)).Identity);
                elt.Add(productElement);
            }
        }
    }
}
