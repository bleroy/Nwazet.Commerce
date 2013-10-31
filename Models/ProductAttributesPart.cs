using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributesPart : ContentPart<ProductAttributesPartRecord> {
        public IEnumerable<int> AttributeIds {
            get {
                var attributes = Retrieve(r => r.Attributes);
                return attributes == null
                    ? new int[0]
                    : attributes
                        .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse);
            }
            set {
                Store(r => r.Attributes, value == null
                    ? null
                    : String.Join(",", value));
            }
        }
    }
}
