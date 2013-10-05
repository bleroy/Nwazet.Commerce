using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IProductAttributeService : IDependency {
        IEnumerable<ProductAttributePart> Attributes { get; }
        IEnumerable<ProductAttributePart> GetAttributes(IEnumerable<int> ids);
    }
}