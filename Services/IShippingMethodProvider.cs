using System.Collections.Generic;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IShippingMethodProvider : IDependency {
        string Name { get; }
        string ContentTypeName { get; }

        IEnumerable<IShippingMethod> GetShippingMethods();
    }
}
