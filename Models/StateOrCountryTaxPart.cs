using System;
using System.Collections.Generic;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Taxes")]
    public class StateOrCountryTaxPart : InfosetContentPart<StateOrCountryTaxPartRecord>, ITax {
        public string State {
            get { return Get(r => r.State); }
            set { Set(r => r.State, value); }
        }

        public string Country {
            get { return Get(r => r.Country); }
            set { Set(r => r.Country, value); }
        }

        public double Rate {
            get { return Get(r => r.Rate); }
            set { Set(r => r.Rate, value); }
        }

        public int Priority {
            get { return Get(r => r.Priority); }
            set { Set(r => r.Priority, value); }
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
            var sameState = !String.IsNullOrWhiteSpace(State) &&
                            (State == "*" || State.Equals(state, StringComparison.CurrentCultureIgnoreCase));
            var sameCountry = !String.IsNullOrWhiteSpace(Country) &&
                              (Country == "*" || Country.Equals(country, StringComparison.CurrentCultureIgnoreCase));
            if (sameState && sameCountry) return tax;
            return 0;
        }
    }
}
