using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;

namespace Nwazet.Commerce.Services {
    public interface IBundleService : IDependency {
        //now returns strings for errors and warnings
        UpdateBundleResults UpdateBundleProducts(ContentItem item, IEnumerable<ProductEntry> products);
        IEnumerable<IContent> GetProducts();
        IEnumerable<ProductPartQuantity> GetProductQuantitiesFor(BundlePart bundle);
        BundleViewModel BuildEditorViewModel(BundlePart part);
        void AddProduct(int quantity, int product, BundlePartRecord record);
    }

    public class UpdateBundleResults {

        public List<LocalizedString> Warnings { get; set; }
        public List<LocalizedString> Errors { get; set; }

        public UpdateBundleResults() {
            Warnings = new List<LocalizedString>();
            Errors = new List<LocalizedString>();
        }
    }
}
