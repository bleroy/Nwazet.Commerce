using System.Collections.Generic;
using Nwazet.Commerce.Services;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.ViewModels {
    [OrchardFeature("Nwazet.Taxes")]
    public class TaxIndexViewModel {
        public IList<ITaxProvider> TaxProviders { get; set; }
        public IList<ITax> Taxes { get; set; }
        public dynamic Pager { get; set; }
    }
}
