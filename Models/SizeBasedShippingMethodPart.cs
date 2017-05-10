using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Shipping")]
    public class SizeBasedShippingMethodPart : ContentPart<SizeBasedShippingMethodPartRecord>, IShippingMethod {
        public string Name {
            get { return Retrieve(r => r.Name); }
            set { Store(r => r.Name, value); }
        }

        public string ShippingCompany {
            get { return Retrieve(r => r.ShippingCompany); }
            set { Store(r => r.ShippingCompany, value); }
        }

        public decimal Price {
            get { return Retrieve(r => r.Price); }
            set { Store(r => r.Price, value); }
        }

        public string Size {
            get { return Retrieve(r => r.Size); }
            set { Store(r => r.Size, value); }
        }

        public int Priority {
            get { return Retrieve(r => r.Priority); }
            set { Store(r => r.Priority, value); }
        }

        public string IncludedShippingAreas {
            get { return Retrieve(r => r.IncludedShippingAreas); }
            set { Store(r => r.IncludedShippingAreas, value); }
        }

        public string ExcludedShippingAreas {
            get { return Retrieve(r => r.ExcludedShippingAreas); }
            set { Store(r => r.ExcludedShippingAreas, value); }
        }

        public IEnumerable<ShippingOption> ComputePrice(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            IEnumerable<IShippingMethod> shippingMethods,
            string country,
            string zipCode,
            IWorkContextAccessor workContextAccessor) {

            // Get all size-based shipping methods
            var sizePriorities = shippingMethods
                .Where(m => m.GetType() == typeof (SizeBasedShippingMethodPart))
                .Cast<SizeBasedShippingMethodPart>()
                .Where(m => !string.IsNullOrWhiteSpace(m.Size))
                .GroupBy(m => m.Size)
                .ToDictionary(g => g.Key, g => g.Min(m => m.Priority));
            var quantities = productQuantities.ToList();
            var fixedCost = quantities
                .Where(pq => pq.Product.ShippingCost != null && pq.Product.ShippingCost >= 0 && !pq.Product.IsDigital)
            // ReSharper disable PossibleInvalidOperationException
                .Sum(pq => pq.Quantity * (decimal)pq.Product.ShippingCost);
            // ReSharper restore PossibleInvalidOperationException
            var relevantQuantities = quantities
                .Where(pq => (pq.Product.ShippingCost == null || pq.Product.ShippingCost < 0) && !pq.Product.IsDigital)
                .ToList();
            // If all products have fixed shipping cost, just return that
            if (relevantQuantities.Count == 0) {
                yield return GetOption(fixedCost);
            }
            // If this is the default size method and no product have a specific size constraint, apply
            else if (string.IsNullOrWhiteSpace(Size) &&
                relevantQuantities.All(pq => string.IsNullOrWhiteSpace(pq.Product.Size))) {

                yield return GetOption(fixedCost + Price);
            }
            // If all products have no specific size or there is a product with a size that has higher priority, pass
            else if (relevantQuantities.Any(pq => !string.IsNullOrWhiteSpace(pq.Product.Size) &&
                pq.Product.Size != Size &&
                sizePriorities.ContainsKey(pq.Product.Size) &&
                sizePriorities[pq.Product.Size] > Priority)) yield break;
            // If no product has the size required by this method, pass
            else if (relevantQuantities.Any(pq => pq.Product.Size == Size)) {
                yield return GetOption(fixedCost + Price);
            }
        }

        private ShippingOption GetOption(decimal price) {
            return new ShippingOption {
                Description = Name,
                Price = price,
                IncludedShippingAreas = IncludedShippingAreas == null ? new string[] { } : IncludedShippingAreas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                ExcludedShippingAreas = ExcludedShippingAreas == null ? new string[] { } : ExcludedShippingAreas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            };
        }
    }
}
