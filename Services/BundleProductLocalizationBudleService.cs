using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.BundlesLocalizationExtension")]
    public class BundleProductLocalizationBudleService : IBundleService {
        public void AddProduct(int quantity, int product, BundlePartRecord record) {
            throw new NotImplementedException();
        }

        public BundleViewModel BuildEditorViewModel(BundlePart part) {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductPartQuantity> GetProductQuantitiesFor(BundlePart bundle) {
            throw new NotImplementedException();
        }

        public IEnumerable<IContent> GetProducts() {
            throw new NotImplementedException();
        }

        public void UpdateBundleProducts(ContentItem item, IEnumerable<ProductEntry> products) {
            throw new NotImplementedException();
        }
    }
}
