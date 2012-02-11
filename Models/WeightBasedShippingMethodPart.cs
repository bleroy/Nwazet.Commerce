using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Shipping")]
    public class WeightBasedShippingMethodPart : ContentPart<WeightBasedShippingMethodPartRecord>, IShippingMethod {
        public string Name { get { return Record.Name; } set { Record.Name = value; } }
        public string ShippingCompany { get { return Record.ShippingCompany; } set { Record.ShippingCompany = value; } }
        public double Price { get { return Record.Price; } set { Record.Price = value; } }
        public double? MinimumWeight { get { return Record.MinimumWeight; } set { Record.MinimumWeight = value; } }
        public double? MaximumWeight { get { return Record.MaximumWeight; } set { Record.MaximumWeight = value; } } // Set to double.PositiveInfinity (the default) for unlimited weight ranges
        public string IncludedShippingAreas { get { return Record.IncludedShippingAreas; } set { Record.IncludedShippingAreas = value; } }
        public string ExcludedShippingAreas { get { return Record.ExcludedShippingAreas; } set { Record.ExcludedShippingAreas = value; } }

        public double ComputePrice(IEnumerable<ShoppingCartQuantityProduct> productQuantities) {
            var fixedCost = productQuantities
                .Where(pq => pq.Product.ShippingCost != null && pq.Product.ShippingCost >= 0 && !pq.Product.IsDigital)
                .Sum(pq => pq.Quantity * (double)pq.Product.ShippingCost);
            var weight = productQuantities
                .Where(pq => (pq.Product.ShippingCost == null || pq.Product.ShippingCost < 0) && !pq.Product.IsDigital)
                .Sum(pq => pq.Quantity * pq.Product.Weight);
            if (weight.CompareTo(0) == 0) return fixedCost;
            if (weight < MinimumWeight || weight > MaximumWeight) return -1; // Shipping method not applicable to this weight range
            return fixedCost + Price;
        }
    }
}
