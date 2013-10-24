using System;
using System.Collections.Generic;
using Nwazet.Commerce.Services;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Taxes")]
    public class StateOrCountryTaxPart : InfosetContentPart<StateOrCountryTaxPartRecord>, ITax {
        public string State {
            get { return Retrieve(r => r.State); }
            set { Store(r => r.State, value); }
        }

        public string Country {
            get { return Retrieve(r => r.Country); }
            set { Store(r => r.Country, value); }
        }

        public double Rate {
            get { return Retrieve(r => r.Rate); }
            set { Store(r => r.Rate, value); }
        }

        public int Priority {
            get { return Retrieve(r => r.Priority); }
            set { Store(r => r.Priority, value); }
        }

        public string Name {
            get {
                var zone = string.IsNullOrWhiteSpace(State) ? Country : Country + " " + State;
                return zone + " (" + Rate.ToString("P") + ")";
            }
        }

        public double ComputeTax(IEnumerable<ShoppingCartQuantityProduct> productQuantities, double subtotal,
            double shippingCost, string country, string zipCode) {

            var tax = (subtotal + shippingCost)*Rate;
            var state = UnitedStates.State(zipCode);
            var sameState = String.IsNullOrWhiteSpace(State) ||
                            State == "*" ||
                            State != null && State.Equals(state ?? "", StringComparison.CurrentCultureIgnoreCase);
            var sameCountry = !String.IsNullOrWhiteSpace(Country) &&
                              (Country == "*" || Country.Equals(country, StringComparison.CurrentCultureIgnoreCase));
            if (sameState && sameCountry) return tax;
            return 0;
        }
    }
}
