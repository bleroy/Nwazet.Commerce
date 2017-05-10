using Orchard;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Services {
    public class ProductInventoryService : ProductInventoryServiceBase {

        public ProductInventoryService(
            IWorkContextAccessor workContextAccessor,
            IContentManager contentManager)
            : base(workContextAccessor, contentManager) { }
    }
}
