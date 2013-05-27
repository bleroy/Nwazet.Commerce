using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Services;
using Orchard;
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

        public IEnumerable<ShippingOption> ComputePrice(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            IEnumerable<IShippingMethod> shippingMethods,
            string country,
            string zipCode,
            IWorkContextAccessor workContextAccessor) {

            var quantities = productQuantities.ToList();
            var fixedCost = quantities
                .Where(pq => pq.Product.ShippingCost != null && pq.Product.ShippingCost >= 0 && !pq.Product.IsDigital)
// ReSharper disable PossibleInvalidOperationException
                .Sum(pq => pq.Quantity * (double)pq.Product.ShippingCost);
// ReSharper restore PossibleInvalidOperationException
            var weight = quantities
                .Where(pq => (pq.Product.ShippingCost == null || pq.Product.ShippingCost < 0) && !pq.Product.IsDigital)
                .Sum(pq => pq.Quantity * pq.Product.Weight);
            if (weight.CompareTo(0) == 0) {
                yield return GetOption(fixedCost);
            }
            else if (weight >= MinimumWeight && weight <= MaximumWeight) {
                yield return GetOption(fixedCost + Price);
            }
        }

        private ShippingOption GetOption(double price) {
            return new ShippingOption {
                Description = Name,
                Price = price,
                IncludedShippingAreas = IncludedShippingAreas == null ? new string[] { } : IncludedShippingAreas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                ExcludedShippingAreas = ExcludedShippingAreas == null ? new string[] { } : ExcludedShippingAreas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            };
        }
    }
}
