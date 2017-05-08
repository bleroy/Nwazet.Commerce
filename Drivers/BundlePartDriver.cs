using System.Globalization;
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

        protected override string Prefix
        {
            get { return "Bundle"; }
        }

        protected override DriverResult Display(BundlePart part, string displayType, dynamic shapeHelper) {
            return ContentShape(
                "Parts_Bundle",
                () => {
                    var products = _bundleService.GetProductQuantitiesFor(part);
                    var productShapes = products.Select(
                        p => {
                            var contentShape = _contentManager.BuildDisplay(p.Product, "Thumbnail").Quantity(p.Quantity);
                            // Also copy quantity onto all shapes under the Content zone
                            foreach (dynamic shape in contentShape.Content.Items) {
                                shape.Quantity(p.Quantity);
                            }
                            return contentShape;
                        });
                    return shapeHelper.Parts_Bundle(
                        ContentPart: part,
                        Products: productShapes);
                });
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
            if (updater.TryUpdateModel(model, Prefix, null, null)) {
                if (part.ContentItem.Id != 0) {
                    _bundleService.UpdateBundleProducts(part.ContentItem, model.Products);
                }
            }

            return Editor(part, shapeHelper);
        }

        protected override void Importing(BundlePart part, ImportContentContext context) {
            var xElement = context.Data.Element("BundlePart");
            if (xElement != null) {
                var productQuantities =
                    xElement.Elements("Product")
                           .Select(e => new ProductQuantity {
                               ProductId = context.GetItemFromSession(e.Attribute("id").Value).Id,
                               Quantity = int.Parse(e.Attribute("quantity").Value, CultureInfo.InvariantCulture)
                           });
                foreach (var productQuantity in productQuantities) {
                    _bundleService.AddProduct(productQuantity.Quantity, productQuantity.ProductId, part.Record);
                }
            }
        }

        protected override void Exporting(BundlePart part, ExportContentContext context) {
            var elt = context.Element("BundlePart");
            foreach (var productQuantity in part.ProductQuantities) {
                var productElement = new XElement("Product");
                productElement.SetAttributeValue("id",
                    context.ContentManager.GetItemMetadata(
                    context.ContentManager.Get(productQuantity.ProductId)).Identity);
                productElement.SetAttributeValue("quantity",
                    productQuantity.Quantity.ToString(CultureInfo.InvariantCulture));
                elt.Add(productElement);
            }
        }
    }
}
