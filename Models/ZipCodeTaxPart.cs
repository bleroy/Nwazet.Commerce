using System;
using System.Collections.Generic;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Taxes")]
    public class ZipCodeTaxPart : ContentPart, ITax {

        public string Name {
            get { return this.Retrieve(r => r.Name); }
            set { this.Store(r => r.Name, value); }
        }

        public int Priority {
            get { return this.Retrieve(r => r.Priority); }
            set { this.Store(r => r.Priority, value); }
        }

        [RegularExpression(@"^(\d{5}(,\s?|\t)0?\.\d*\r*\n*)*$", 
            ErrorMessage="Invalid rate format. Requires comma or tab seperated zip and rate values, one per line.")]
        public string Rates {
            get { return this.Retrieve(r => r.Rates); }
            set { this.Store(r => r.Rates, value); }
        }

        public Dictionary<string, decimal> GetRates() {

            var rates = new Dictionary<string, decimal>();
            string[] rateLines = Rates.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var rateLine in rateLines) {
                var rateSplit = rateLine.Split(new string[] { ",", "\t" }, StringSplitOptions.None);
                rates.Add(rateSplit[0], Convert.ToDecimal(rateSplit[1]));
            }

            return rates;
        }

        public decimal ComputeTax(IEnumerable<ShoppingCartQuantityProduct> productQuantities, decimal subtotal,
            decimal shippingCost, string country, string zipCode) {

            var rates = GetRates();
            if (country == Country.UnitedStates && rates.ContainsKey(zipCode)) {
                var rate = rates[zipCode];
                var tax = (subtotal + shippingCost) * rate;
                return tax;
            }

            return 0;
        }
    }
}
