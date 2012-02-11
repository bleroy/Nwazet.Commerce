using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IShippingAreaProvider : IDependency {
        IEnumerable<ShippingArea> GetAreas();
    }
}
