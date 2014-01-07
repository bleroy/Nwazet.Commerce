using Orchard.Environment.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributeValue {
        [Required]
        public string Text { get; set; }
        [Required]
        public double PriceAdjustment { get; set; }

        public static IEnumerable<ProductAttributeValue> DeserializeAttributeValues(string attributeValues) {
            if (attributeValues != null) {
                return attributeValues.Split(new[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Split('=')).Select(av => new ProductAttributeValue() {
                        Text = av[0],
                        PriceAdjustment = Convert.ToDouble(av[1])
                    })
                    .OrderBy(t => t.Text)
                    .ToList();
            }
            else {
                return new List<ProductAttributeValue>();
            }
        }

        public static string SerializeAttributeValues(IEnumerable<ProductAttributeValue> attributeValues) {
            if (attributeValues != null) {
                return string.Join(";", attributeValues.Select(a => a.Text + "=" + a.PriceAdjustment));
            }
            else {
                return string.Empty;
            }
        }
    }
}
