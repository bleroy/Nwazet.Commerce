using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;

namespace Nwazet.Commerce.Services {
    public class ProductAttributeService : IProductAttributeService {
        private readonly IContentManager _contentManager;

        private IDictionary<int, ProductAttributePart> _attributes; 

        public ProductAttributeService(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        private IDictionary<int, ProductAttributePart> AttributesById {
            get {
                if (_attributes != null) return _attributes;
                return _attributes = _contentManager
                    .Query<ProductAttributePart, ProductAttributePartRecord>(VersionOptions.Published)
                    .WithQueryHints(new QueryHints().ExpandParts<TitlePart>())
                    .List()
                    .ToDictionary(a => a.Id, a => a);
            }
        }

        public IEnumerable<ProductAttributePart> Attributes {
            get {
                return AttributesById.Values;
            }
        }

        public IEnumerable<ProductAttributePart> GetAttributes(IEnumerable<int> ids) {
            var attributes = AttributesById;
            return ids.Select(id => attributes[id]).ToList();
        }
    }
}