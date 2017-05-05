using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Services {
    public interface IBundleService : IDependency {
        void UpdateBundleProducts(ContentItem item, IEnumerable<ProductEntry> products);
        IEnumerable<IContent> GetProducts();
        IEnumerable<ProductPartQuantity> GetProductQuantitiesFor(BundlePart bundle);
        BundleViewModel BuildEditorViewModel(BundlePart part);
        void AddProduct(int quantity, int product, BundlePartRecord record);
    }
}
