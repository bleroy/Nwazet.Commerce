using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.Settings;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.UI.Notify;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Bundles")]
    public class BundlePartDriver : ContentPartDriver<BundlePart> {
        private readonly IBundleService _bundleService;
        private readonly IBundleAutocompleteService _bundleAutocompleteService;
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _orchardServices;
        public BundlePartDriver(
            IBundleService bundleService,
            IBundleAutocompleteService bundleAutocompleteService,
            IContentManager contentManager,
            IOrchardServices orchardServices) {

            _bundleService = bundleService;
            _bundleAutocompleteService = bundleAutocompleteService;
            _contentManager = contentManager;
            _orchardServices = orchardServices;
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
           if (part.TypePartDefinition.Settings.GetModel<BundleProductSettings>().Autocomplete) {
                return ContentShape("Parts_Bundle_Autocomplete_Edit",
                    () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/BundleAutocomplete",
                    Model: _bundleAutocompleteService.BuildEditorViewModel(part),
                    Prefix: Prefix));
            }
           else
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
                    var updateResults = _bundleService.UpdateBundleProducts(part.ContentItem, model.Products);
                    foreach (var error in updateResults.Errors) {
                        updater.AddModelError("", error);
                    }
                    foreach (var warning in updateResults.Warnings) {
                        _orchardServices.Notifier.Warning(warning);
                    }
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
