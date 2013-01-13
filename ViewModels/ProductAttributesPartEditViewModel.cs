using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.ViewModels {
    public class ProductAttributesPartEditViewModel {
        public IEnumerable<IContent> Attributes { get; set; }
        public ProductAttributesPart Part { get; set; }
        public string Prefix { get; set; }
    }
}
