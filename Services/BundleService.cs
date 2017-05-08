using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.MediaLibrary.Models;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Bundles")]
    public class BundleService : BundleServiceBase {
        public BundleService(
            IContentManager contentManager,
            IRepository<BundleProductsRecord> bundleProductsRepository) 
            : base(contentManager, bundleProductsRepository) { }
        
    }
}
