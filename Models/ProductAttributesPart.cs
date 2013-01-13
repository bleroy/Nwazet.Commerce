using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributesPart: ContentPart<ProductAttributesPartRecord> {
        public IEnumerable<int> AttributeIds {
            get {
                return Record.Attributes == null 
                    ? new int[0] 
                    : Record.Attributes
                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse);
            }
            set {
                Record.Attributes = value == null
                                             ? null
                                             : String.Join(",", value);
            }
        }
    }
}
