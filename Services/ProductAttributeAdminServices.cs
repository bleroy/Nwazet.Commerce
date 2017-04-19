using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributeAdminServices : IProductAttributeAdminServices {
        private readonly IContentManager _contentManager;

        public ProductAttributeAdminServices(
            IContentManager contentManager) {

            _contentManager = contentManager;
        }

        public IEnumerable<ProductAttributePart> GetAllParts() {
            return _contentManager
                .Query<ProductAttributePart>()
                .Join<TitlePartRecord>()
                .OrderBy(p => p.Title)
                .List();
        }
    }
}
