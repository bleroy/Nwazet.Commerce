using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributePart : InfosetContentPart<ProductAttributePartRecord> {
        public IEnumerable<string> AttributeValues {
            get {
                var values = AttributeValuesString;
                return values == null
                    ? new string[0]
                    : values
                        .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(v => v.Trim());
            }
            set {
                AttributeValuesString = value == null
                    ? null
                    : String.Join("\r\n", value.Select(v => v.Trim()));
            }
        }

        internal string AttributeValuesString {
            get {
                return Get(r => r.AttributeValues);
            }
            set {
                Set(r => r.AttributeValues, value);
            }
        }
    }
}
