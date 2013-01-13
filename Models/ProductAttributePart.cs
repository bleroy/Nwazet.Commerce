using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributePart: ContentPart<ProductAttributePartRecord> {
        public IEnumerable<string> AttributeValues {
            get {
                return Record.AttributeValues == null
                    ? new string[0]
                    : Record.AttributeValues.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }
            set {
                Record.AttributeValues = value == null
                                             ? null
                                             : String.Join("\r\n", value);
            }
        }
    }
}
