using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Services {
    public interface IProductAttributesDriver : IDependency {
        dynamic GetAttributeDisplayShape(IContent product, dynamic shapeHelper);
        bool ValidateAttributes(IContent product, IDictionary<int, ProductAttributeValueExtended> attributeIdsToValues);
    }
}
