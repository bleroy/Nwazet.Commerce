using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.ViewModels {
    public class AttributeLocalizationProductAttributesPartEditViewModel {
        public IEnumerable<IContent> AttributesToHide { get; set; }
        public IEnumerable<IContent> AttributesToMark { get; set; }
        public ProductAttributesPart Part { get; set; }
        public string Prefix { get; set; }
    }
}
