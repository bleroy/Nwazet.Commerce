using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatConfigurationPart : ContentPart<VatConfigurationPartRecord>, ITax {

        #region ITax implementation
        public string Name {
            get { return "VAT"; }
        }

        public int Priority {
            get { return Retrieve(r => r.Priority); }
            set { Store(r => r.Priority, value); }
        }

        public decimal ComputeTax(
            IEnumerable<ShoppingCartQuantityProduct> productQuantities,
            decimal subtotal, decimal shippingCost, string country, string zipCode) {

            // tax computations are too complex to reliably do here without services and such
            // The VatTaxComputationHelper will make sure this method is not called.
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// This would be the product category this VAT will apply to. Uniqueness of this will have to be
        /// enforced in code, because the migrations fail if we attempt to set this (actually, the corresponding
        /// value in the record) as unique.
        /// </summary>
        public string TaxProductCategory {
            get { return Retrieve(r => r.TaxProductCategory) ?? string.Empty; }
            set { Store(r => r.TaxProductCategory, value ?? string.Empty); }
        }

        /// <summary>
        /// Default rate for territories outside the selected hierarchies
        /// </summary>
        public decimal DefaultRate {
            get { return Retrieve(r => r.DefaultRate); }
            set { Store(r => r.DefaultRate, value); }
        }

        private LazyField<IEnumerable<Tuple<TerritoryHierarchyPart, decimal>>> _hierarchies =
            new LazyField<IEnumerable<Tuple<TerritoryHierarchyPart, decimal>>>();

        public LazyField<IEnumerable<Tuple<TerritoryHierarchyPart, decimal>>> HierarchiesField {
            get { return _hierarchies; }
        }

        public IEnumerable<Tuple<TerritoryHierarchyPart, decimal>> Hierarchies {
            get { return _hierarchies.Value; }
        }

        private LazyField<IEnumerable<Tuple<TerritoryPart, decimal>>> _territories =
            new LazyField<IEnumerable<Tuple<TerritoryPart, decimal>>>();

        public LazyField<IEnumerable<Tuple<TerritoryPart, decimal>>> TerritoriesField {
            get { return _territories; }
        }

        public IEnumerable<Tuple<TerritoryPart, decimal>> Territories {
            get { return _territories.Value; }
        }
    }
}