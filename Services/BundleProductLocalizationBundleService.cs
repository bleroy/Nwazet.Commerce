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
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

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
        //public override void UpdateBundleProducts(ContentItem item, IEnumerable<ProductEntry> products) {
        //    throw new NotImplementedException();
        //}
    }
}
