using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributesPart : InfosetContentPart<ProductAttributesPartRecord> {
        public IEnumerable<int> AttributeIds {
            get {
                var attributes = Get(r => r.Attributes);
                return attributes == null
                    ? new int[0]
                    : attributes
                        .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse);
            }
            set {
                Set(r => r.Attributes, value == null
                    ? null
                    : String.Join(",", value));
            }
        }
    }
}
