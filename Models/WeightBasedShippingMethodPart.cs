using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Shipping")]
    public class WeightBasedShippingMethodPart : InfosetContentPart<WeightBasedShippingMethodPartRecord>,
        IShippingMethod {
        public string Name {
            get { return Get(r => r.Name); }
            set { Set(r => r.Name, value); }
        }

        public string ShippingCompany {
            get { return Get(r => r.ShippingCompany); }
            set { Set(r => r.ShippingCompany, value); }
        }

        public double Price {
            get { return Get(r => r.Price); }
            set { Set(r => r.Price, value); }
        }

        public string IncludedShippingAreas {
            get { return Get(r => r.IncludedShippingAreas); }
            set { Set(r => r.IncludedShippingAreas, value); }
        }

        public string ExcludedShippingAreas {
            get { return Get(r => r.ExcludedShippingAreas); }
            set { Set(r => r.ExcludedShippingAreas, value); }
        }

        public double? MinimumWeight {
            get { return Get(r => r.MinimumWeight); }
            set { Set(r => r.MinimumWeight, value); }
        }

        public double? MaximumWeight {
            get { return Get(r => r.MaximumWeight); }
            set { Set(r => r.MaximumWeight, value); }
        } // Set to double.PositiveInfinity (the default) for unlimited weight ranges

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
                .Sum(pq => pq.Quantity*(double) pq.Product.ShippingCost);
// ReSharper restore PossibleInvalidOperationException
            var weight = quantities
                .Where(pq => (pq.Product.ShippingCost == null || pq.Product.ShippingCost < 0) && !pq.Product.IsDigital)
                .Sum(pq => pq.Quantity*pq.Product.Weight);
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
                IncludedShippingAreas =
                    IncludedShippingAreas == null
                        ? new string[] {}
                        : IncludedShippingAreas.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries),
                ExcludedShippingAreas =
                    ExcludedShippingAreas == null
                        ? new string[] {}
                        : ExcludedShippingAreas.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
            };
        }
    }
}
