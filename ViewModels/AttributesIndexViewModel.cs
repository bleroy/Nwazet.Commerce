using System.Collections.Generic;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.ViewModels {
    public class AttributesIndexViewModel {
        public IEnumerable<ProductAttributePart> Attributes { get; set; }
        public dynamic Pager { get; set; }
    }
}
