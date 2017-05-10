using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Settings;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Localization.Models;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.BundlesLocalizationExtension")]
    public class BundleProductLocalizationBundleService : BundleServiceBase {

        private readonly IBundleProductLocalizationServices _bundleProductLocalizationServices;
        public BundleProductLocalizationBundleService(
            IContentManager contentManager,
            IRepository<BundleProductsRecord> bundleProductsRepository,
            IBundleProductLocalizationServices bundleProductLocalizationServices)
            : base(contentManager, bundleProductsRepository) {

            _bundleProductLocalizationServices = bundleProductLocalizationServices;
        }

        protected override bool ConsiderProductValid(IContent prod, BundlePart part) {
            var settings = part.TypePartDefinition.Settings.GetModel<BundleProductLocalizationSettings>();
            var bundleProductQuantities = part.ProductQuantities.ToDictionary(pq => pq.ProductId, pq => pq.Quantity);
            LocalizationPart locPart = part.ContentItem.As<LocalizationPart>();
            return //Conditions under which we consider the product for the bundle:
                !settings.HideProductsFromEditor || //if we should not hide away products, they are all fine
                !_bundleProductLocalizationServices.ValidLocalizationPart(locPart) || //if the bundle does not have a valid LocalizationPart, I have no criteria to hide products
                bundleProductQuantities.ContainsKey(prod.ContentItem.Id) || //the product is in the bundle, so don't hide it
                (
                    settings.HideProductsFromEditor && //should hide products in the "wrong" culture
                    !_bundleProductLocalizationServices.HasDifferentCulture(prod, locPart) //keep the products whose culture is not "wrong"
                );
        }

        public override BundleViewModel BuildEditorViewModel(BundlePart part) {
            var bvm = base.BuildEditorViewModel(part);
            LocalizationPart locPart = part.ContentItem.As<LocalizationPart>();
            if (_bundleProductLocalizationServices.ValidLocalizationPart(locPart)) {
                List<ProductEntry> oldProducts = bvm.Products.ToList();
                bvm.Products = new List<ProductEntry>();
                foreach (var prodEntry in oldProducts) {
                    var item = _contentManager.Get(prodEntry.ProductId);
                    var lPart = item.As<LocalizationPart>();
                    if (_bundleProductLocalizationServices.ValidLocalizationPart(lPart)) {
                        if (lPart.Culture != locPart.Culture) {
                            prodEntry.DisplayText += T(" ({0})", lPart.Culture.Culture);
                        }
                    }
                    else {
                        prodEntry.DisplayText += T(" (culture undefined)");
                    }
                    bvm.Products.Add(prodEntry);
                }
            }
            return bvm;
        }

        private string DisplayTextFromId(int id) {
            return _contentManager.GetItemMetadata(_contentManager.GetLatest(id)).DisplayText;
        }

        public override UpdateBundleResults UpdateBundleProducts(ContentItem item, IEnumerable<ProductEntry> products) {
            //localization logic
            var locPart = item.As<LocalizationPart>();
            if (!_bundleProductLocalizationServices.ValidLocalizationPart(locPart)) {
                return base.UpdateBundleProducts(item, products);
            }
            //LocalizationPart in the ContentItem has the info we need
            var bundlePart = item.As<BundlePart>();
            var results = new UpdateBundleResults();
            var record = bundlePart.Record;
            var oldProducts = _bundleProductsRepository
                .Fetch(r => r.BundlePartRecord == record)
                .ToList();

            var settings = bundlePart.TypePartDefinition.Settings.GetModel<BundleProductLocalizationSettings>();

            var newProductsList = settings.TryToLocalizeProducts ?
                _bundleProductLocalizationServices.GetLocalizationIdPairs(
                    products
                    .Where(e => e.Quantity > 0)
                    .Select(pe => new ProductQuantity() {
                        ProductId = pe.ProductId,
                        Quantity = pe.Quantity
                    }), locPart) :
                products
                    .Where(e => e.Quantity > 0)
                    .Select(pr => new ProductQuantityPair(
                        op: new ProductQuantity { ProductId = pr.ProductId, Quantity = pr.Quantity },
                        np: pr.ProductId
                    ));

            if (settings.TryToLocalizeProducts) {
                //try to replace selected products with the correct localization
                if (newProductsList.Any(pqp => pqp.NewProductId < 0)) {
                    if (settings.RemoveProductsWithoutLocalization) {
                        results.Warnings.Add(T(
                            "We could not find a correct localization for the following products, so they were removed from this bundle: {0}",
                            string.Join(", ", newProductsList.Where(pqp => pqp.NewProductId < 0)
                                .Select(pqp => DisplayTextFromId(pqp.OriginalProduct.ProductId)
                                )
                            )
                        ));
                        newProductsList = newProductsList.Where(pqp => pqp.NewProductId > 0);
                    }
                    else {
                        //negative Ids are made positive again
                        newProductsList = newProductsList.Select(pqp =>
                            pqp = new ProductQuantityPair(pqp.OriginalProduct, Math.Abs(pqp.NewProductId)));
                    }
                }
            }

            if (settings.AssertProductsHaveSameCulture) {
                //verify that all products are in the same culture as the bundle
                var badProducts = _bundleProductLocalizationServices
                    .GetProductsInTheWrongCulture(newProductsList.Select(pqp => pqp.NewProductId), locPart);
                if (badProducts.Any()) {
                    results.Errors.Add(T("Some of the products are in the wrong culture: {0}",
                        string.Join(", ", badProducts.Select(bp => _contentManager.GetItemMetadata(bp).DisplayText))
                        ));
                    return results;
                }
            }
            //generate the list of new product quantities
            Dictionary<int, int> newQuantities = new Dictionary<int, int>(); //Dictionary<TProductId, TQuantity>
            foreach (var quantityPair in newProductsList) {
                if (newQuantities.ContainsKey(quantityPair.NewProductId)) {
                    if (settings.TryToLocalizeProducts && settings.AddProductQuantitiesWhenLocalizing) {
                        newQuantities[quantityPair.NewProductId] += quantityPair.OriginalProduct.Quantity;
                    }
                    else if (quantityPair.NewProductId == quantityPair.OriginalProduct.ProductId) {
                        newQuantities[quantityPair.NewProductId] = quantityPair.OriginalProduct.Quantity;
                    }
                }
                else {
                    newQuantities.Add(quantityPair.NewProductId, quantityPair.OriginalProduct.Quantity);
                }
            }
            //assign
            var innerResults = base.UpdateBundleProducts(item,
                newQuantities.Select(kvp => new ProductEntry {
                    ProductId = kvp.Key,
                    Quantity = kvp.Value
                }));
            results.Warnings.AddRange(innerResults.Warnings);
            results.Errors.AddRange(innerResults.Errors);
            //notify
            if (newProductsList.Where(pqp => pqp.OriginalProduct.ProductId != pqp.NewProductId).Any()) {
                results.Warnings.Add(T(
                       "The following products where replaced by their correct localization: {0}",
                       string.Join(", ", newProductsList.Where(pqp => pqp.OriginalProduct.ProductId != pqp.NewProductId)
                           .Select(pqp => DisplayTextFromId(pqp.OriginalProduct.ProductId)
                           )
                       )
                   ));
            }

            return results; //no error
        }
    }
}
