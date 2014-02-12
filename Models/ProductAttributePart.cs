using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributePart : ContentPart<ProductAttributePartRecord> {
        public IEnumerable<ProductAttributeValue> AttributeValues {
            get {
                return ProductAttributeValue.DeserializeAttributeValues(AttributeValuesString);
            }
            set {
                AttributeValuesString = ProductAttributeValue.SerializeAttributeValues(value);
            }
        }

        internal string AttributeValuesString {
            get {
                return Retrieve(r => r.AttributeValues);
            }
            set {
                Store(r => r.AttributeValues, value);
            }
        }
    }
}
