using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributePart : ContentPart<ProductAttributePartRecord> {
        public IEnumerable<string> AttributeValues {
            get {
                return Record.AttributeValues == null
                    ? new string[0]
                    : Record.AttributeValues
                        .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(v => v.Trim());
            }
            set {
                Record.AttributeValues = value == null
                    ? null
                    : String.Join("\r\n", value.Select(v => v.Trim()));
            }
        }
    }
}
