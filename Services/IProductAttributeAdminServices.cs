using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IProductAttributeAdminServices : IDependency {

        IEnumerable<ProductAttributePart> GetAllProductAttributeParts();
    }
}
