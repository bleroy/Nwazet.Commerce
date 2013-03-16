using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Shipping")]
    public class CoreShippingAreas : IShippingAreaProvider {
        private readonly ShippingArea[] _areas =
            new[] {
                new ShippingArea {Id = "world", Name = "World"},
                new ShippingArea {Id = "canada", Name = "Canada"}, 
                new ShippingArea {Id = "us", Name = "All US states"},
                new ShippingArea {Id = "us-continental", Name = "All US states except Alaska and Hawaii"}
            };

        public IEnumerable<ShippingArea> GetAreas() {
            return _areas;
        }
    }
}
