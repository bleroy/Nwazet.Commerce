using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IBundleAutocompleteService : IDependency {
        BundleViewModel BuildEditorViewModel(BundlePart part);
        List<ProductEntryAutocomplete> GetProducts(string searchtext,List<int> exclude);
    }
}
