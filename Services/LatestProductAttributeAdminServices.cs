using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AttributesLocalizationExtension")]
    public class LatestProductAttributeAdminServices : IProductAttributeAdminServices {
        private readonly IContentManager _contentManager;

        public LatestProductAttributeAdminServices(
            IContentManager contentManager) {

            _contentManager = contentManager;
        }

        public IEnumerable<ProductAttributePart> GetAllProductAttributeParts() {
            return _contentManager
                .Query<ProductAttributePart>(VersionOptions.Latest)
                .Join<TitlePartRecord>()
                .OrderBy(p => p.Title)
                .List();
        }
    }
}
