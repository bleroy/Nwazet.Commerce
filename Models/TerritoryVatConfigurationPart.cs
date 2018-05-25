using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class TerritoryVatConfigurationPart : ContentPart<TerritoryVatConfigurationPartRecord> {

        private LazyField<IEnumerable<Tuple<VatConfigurationPart, decimal>>> _vatConfigurations =
            new LazyField<IEnumerable<Tuple<VatConfigurationPart, decimal>>>();

        public LazyField<IEnumerable<Tuple<VatConfigurationPart, decimal>>> VatConfigurationsField {
            get { return _vatConfigurations; }
        }

        public IEnumerable<Tuple<VatConfigurationPart, decimal>> VatConfigurations {
            get { return _vatConfigurations.Value; }
        }
    }
}
