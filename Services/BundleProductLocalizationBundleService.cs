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

        public BundleProductLocalizationBundleService(
            IContentManager contentManager,
            IRepository<BundleProductsRecord> bundleProductsRepository)
            : base(contentManager, bundleProductsRepository) {

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static bool HasDifferentCulture(IContent ci, LocalizationPart locPart) {
            var lP = ci.As<LocalizationPart>();
            return lP != null && //has a LocalizationPart AND
                (lP.Culture == null || //culture undefined OR
                    (string.IsNullOrWhiteSpace(lP.Culture.Culture) || //culture undefined OR
                        (lP.Culture != locPart.Culture))); //culture different than the product's 
        }

        private static bool ValidLocalizationPart(LocalizationPart part) {
            return part != null &&
                part.Culture != null &&
                !string.IsNullOrWhiteSpace(part.Culture.Culture);
        }

        protected override bool ConsiderProductValid(IContent prod, BundlePart part) {
            var settings = part.TypePartDefinition.Settings.GetModel<BundleProductLocalizationSettings>();
            var bundleProductQuantities = part.ProductQuantities.ToDictionary(pq => pq.ProductId, pq => pq.Quantity);
            LocalizationPart locPart = part.ContentItem.As<LocalizationPart>();
            return //Conditions under which we consider the product for the bundle:
                !settings.HideProductsFromEditor || //if we should not hide away products, they are all fine
                !ValidLocalizationPart(locPart) || //if the bundle does not have a valid LocalizationPart, I have no criteria to hide products
                bundleProductQuantities.ContainsKey(prod.ContentItem.Id) || //the product is in the bundle, so don't hide it
                (
                    settings.HideProductsFromEditor && //should hide products in the "wrong" culture
                    ValidLocalizationPart(locPart) && //our bundle actually has a culture
                    !HasDifferentCulture(prod, locPart) //keep the products whose culture is not "wrong"
                );
        }

        public override BundleViewModel BuildEditorViewModel(BundlePart part) {
            var bvm = base.BuildEditorViewModel(part);
            LocalizationPart locPart = part.ContentItem.As<LocalizationPart>();
            if (ValidLocalizationPart(locPart)) {
                List<ProductEntry> oldProducts = bvm.Products.ToList();
                bvm.Products = new List<ProductEntry>();
                foreach (var prodEntry in oldProducts) {
                    var item = _contentManager.Get(prodEntry.ProductId);
                    var lPart = item.As<LocalizationPart>();
                    if (ValidLocalizationPart(lPart)) {
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
        //public override void UpdateBundleProducts(ContentItem item, IEnumerable<ProductEntry> products) {
        //    throw new NotImplementedException();
        //}
    }
}
