using Nwazet.Commerce.Services;
using Orchard.Environment.Extensions;
using System;
using System.Xml.Serialization;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    [Serializable]
    public class ProductAttributeValueExtended : IEquatable<ProductAttributeValueExtended> {
        public string Value { get; set; }
        public string ExtendedValue { get; set; }
        public string ExtensionProvider { get; set; }
        [NonSerialized]
        public IProductAttributeExtensionProvider ExtensionProviderInstance;

        public bool Equals(ProductAttributeValueExtended other) {
            if (other == null) {
                return false;
            }
            return this.Value == other.Value &&
                this.ExtendedValue == other.ExtendedValue &&
                this.ExtensionProvider == other.ExtensionProvider;
        }

        public override string ToString() {
            return $"({Value}, {ExtendedValue}, {ExtensionProvider})";
        }
    }
}
