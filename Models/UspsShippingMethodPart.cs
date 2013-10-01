using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Usps.Shipping")]
    public class UspsShippingMethodPart : ContentPart<UspsShippingMethodPartRecord>, IShippingMethod {
        public string Name {
            get { return Record.Name; }
            set { Record.Name = value; }
        }

        public string Size {
            get { return Record.Size; }
            set { Record.Size = value; }
        }

        public string Container {
            get { return Record.Container; }
            set { Record.Container = value; }
        }

        public string ContainerSize {
            get { return UspsContainer.List[Record.Container].ToString(); }
        }

        public double Markup {
            get { return Record.Markup; }
            set { Record.Markup = value; }
        }

        public int WidthInInches {
            get { return Record.WidthInInches; }
            set { Record.WidthInInches = value; }
        }

        public int LengthInInches {
            get { return Record.LengthInInches; }
            set { Record.LengthInInches = value; }
        }

        public int HeightInInches {
            get { return Record.HeightInInches; }
            set { Record.HeightInInches = value; }
        }

        public double MaximumWeightInOunces {
            get { return Record.MaximumWeightInOunces; }
            set { Record.MaximumWeightInOunces = value; }
        }

        public double WeightPaddingInOunces {
            get { return Record.WeightPaddingInOunces; }
            set { Record.WeightPaddingInOunces = value; }
        }

        public string ServiceNameValidationExpression {
            get { return Record.ServiceNameValidationExpression; }
            set { Record.ServiceNameValidationExpression = value; }
        }

        public string ServiceNameExclusionExpression {
            get { return Record.ServiceNameExclusionExpression; }
            set { Record.ServiceNameExclusionExpression = value; }
        }

        public int Priority {
            get { return Record.Priority; }
            set { Record.Priority = value; }
        }

        public string ShippingCompany {
            get { return "USPS"; }
            set { throw new InvalidOperationException("ShippingCompany cannot be set."); }
        }

        public string IncludedShippingAreas { get; set; }
        public string ExcludedShippingAreas { get; set; }

        public bool International {
            get { return Record.International; }
            set { Record.International = value; }
        }

        public bool RegisteredMail {
            get { return Record.RegisteredMail; }
            set { Record.RegisteredMail = value; }
        }

        public bool Insurance {
            get { return Record.Insurance; }
            set { Record.Insurance = value; }
        }

        public bool ReturnReceipt {
            get { return Record.ReturnReceipt; }
            set { Record.ReturnReceipt = value; }
        }

        public bool CertificateOfMailing {
            get { return Record.CertificateOfMailing; }
            set { Record.CertificateOfMailing = value; }
        }

        public bool ElectronicConfirmation {
            get { return Record.ElectronicConfirmation; }
            set { Record.ElectronicConfirmation = value; }
        }

        public IEnumerable<ShippingOption> ComputePrice(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            IEnumerable<IShippingMethod> shippingMethods,
            string country,
            string zipCode,
            IWorkContextAccessor workContextAccessor) {

            var quantities = productQuantities.ToList();
            var fixedCost = quantities
                .Where(pq => pq.Product.ShippingCost != null &&
                             pq.Product.ShippingCost >= 0 &&
                             !pq.Product.IsDigital)
                .Sum(pq => pq.Quantity*(double) pq.Product.ShippingCost);
            var relevantQuantities = quantities
                .Where(pq => (pq.Product.ShippingCost == null || pq.Product.ShippingCost < 0) &&
                             !pq.Product.IsDigital)
                .ToList();

            var wc = workContextAccessor.GetContext();
            var uspsService = wc.Resolve<IUspsService>();

            // If all products have fixed shipping cost, just return that
            if (relevantQuantities.Count == 0) {
                var domesticAreas = uspsService.GetDomesticShippingAreas().ToList();
                var internationalAreas = uspsService.GetInternationalShippingAreas().ToList();
                var included = International ? internationalAreas : domesticAreas;
                var excluded = International ? domesticAreas : internationalAreas;

                yield return GetOption(fixedCost, included, excluded);
                yield break;
            }

            var uspsSettings = uspsService.GetSettings();
            var weight = relevantQuantities.Sum(pq => pq.Quantity*pq.Product.Weight*16) + WeightPaddingInOunces;
            // If above the maximum package weight, pass
            if (MaximumWeightInOunces > 0 && weight > MaximumWeightInOunces) yield break;

            var valueOfContents = relevantQuantities.Sum(pq => pq.Quantity*pq.Price);

            var sizePriorities = shippingMethods
                .Where(m => m.GetType() == typeof (UspsShippingMethodPart))
                .Cast<UspsShippingMethodPart>()
                .Where(m => !string.IsNullOrWhiteSpace(m.Size))
                .GroupBy(m => m.Size)
                .ToDictionary(g => g.Key, g => g.Min(m => m.Priority));

            // If all products have no specific size or there is a product with a size that has higher priority, pass
            if (relevantQuantities.Any(pq => !string.IsNullOrWhiteSpace(pq.Product.Size) &&
                                             pq.Product.Size != Size &&
                                             sizePriorities.ContainsKey(pq.Product.Size) &&
                                             sizePriorities[pq.Product.Size] > Priority)) yield break;
            // If no product has the size required by this method, pass
            if (relevantQuantities.All(pq => pq.Product.Size != Size)) yield break;
            // If the destination is not consistent with the method, pass
            if ((International && country == Country.UnitedStates) ||
                (!International && country != Country.UnitedStates) ||
                (!International && String.IsNullOrWhiteSpace(zipCode))) yield break;

            var prices = uspsService.Prices(
                uspsSettings.UserId,
                weight,
                valueOfContents,
                Container,
                ServiceNameValidationExpression,
                ServiceNameExclusionExpression,
                country,
                LengthInInches,
                WidthInInches,
                HeightInInches,
                uspsSettings.OriginZip,
                zipCode,
                uspsSettings.CommercialPrices,
                uspsSettings.CommercialPlusPrices,
                RegisteredMail,
                Insurance,
                ReturnReceipt,
                CertificateOfMailing,
                ElectronicConfirmation);
            foreach (var price in prices) {
                price.Price += fixedCost + Markup;
                yield return price;
            }
        }

        private ShippingOption GetOption(double price, IList<string> includedShippingAreas, IList<string> excludedShippingAreas) {
            return new ShippingOption {
                Description = Name,
                Price = price,
                IncludedShippingAreas = includedShippingAreas,
                ExcludedShippingAreas = excludedShippingAreas
            };
        }
    }
}
