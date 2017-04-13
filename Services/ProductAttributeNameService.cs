using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Extensions;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Utility.Extensions;

namespace Nwazet.Commerce.Services {
    public class ProductAttributeNameService : IProductAttributeNameService {

        private readonly IContentManager _contentManager;
        public ProductAttributeNameService(
            IContentManager contentManager) {
            _contentManager = contentManager;
        }

        private IEnumerable<ProductAttributePart> GetSimilarNames(string tName) {
            return _contentManager
                .Query<ProductAttributePart, ProductAttributePartRecord>()
                .Where(part => part.TechnicalName != null && part.TechnicalName.StartsWith(tName))
                .List();
        }

        public string GenerateAttributeTechnicalName(string displayName) {
            displayName = displayName.ToSafeName();
            var attributes = GetSimilarNames(displayName);
            return AttributeNameUtilities.GenerateAttributeTechnicalName(displayName, attributes);
        }

        public bool ProcessTechnicalName(ProductAttributePart part) {
            bool sameName = true;
            var attributes = GetSimilarNames(part.TechnicalName).Where(p => p.ContentItem.Id != part.ContentItem.Id);
            if (attributes.Any()) {
                var oldName = part.TechnicalName;
                part.TechnicalName = AttributeNameUtilities.GenerateAttributeTechnicalName(part.TechnicalName, attributes);
                if (part.TechnicalName != oldName) {
                    sameName = false;
                }
            }
            return sameName;
        }
    }
}
