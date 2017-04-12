using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System.ComponentModel;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributePart : ContentPart<ProductAttributePartRecord> {
        public IEnumerable<ProductAttributeValue> AttributeValues
        {
            get
            {
                return ProductAttributeValue.DeserializeAttributeValues(AttributeValuesString);
            }
            set
            {
                AttributeValuesString = ProductAttributeValue.SerializeAttributeValues(value);
            }
        }

        [DisplayName("Sort Order")]
        public int SortOrder
        {
            get { return Retrieve(r => r.SortOrder); }
            set { Store(r => r.SortOrder, value); }
        }

        [DisplayName("Display Name")]
        public string DisplayName
        {
            get { return Retrieve(r => r.DisplayName); }
            set { Store(r => r.DisplayName, value); }
        }

        [DisplayName("Technical Name")]
        public string TechnicalName
        {
            get { return Retrieve(r => r.TechnicalName); }
            set { Store(r => r.TechnicalName, value); }
        }

        internal string AttributeValuesString
        {
            get
            {
                return Retrieve(r => r.AttributeValues);
            }
            set
            {
                Store(r => r.AttributeValues, value);
            }
        }
    }
}
