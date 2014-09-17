using Orchard.Environment.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Attributes")]
    public class ProductAttributeValue {
        [Required, DisplayName("Value Name")]
        public string Text { get; set; }
        [Required, DisplayName("Price Adjustment")]
        public double PriceAdjustment { get; set; }
        [DefaultValue(false), DisplayName("Is Line Adjustment")]
        public bool IsLineAdjustment { get; set; }
        [DefaultValue(0), DisplayName("Sort Order")]
        public int SortOrder { get; set; }

        public static IEnumerable<ProductAttributeValue> DeserializeAttributeValues(string attributeValues) {
            if (attributeValues != null) {
                return attributeValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Split('=')).Select(av => {
                        var attrSettings = av[1].Split(',');
                        return new ProductAttributeValue {
                            Text = av[0],
                            PriceAdjustment = Convert.ToDouble(attrSettings[0]),
                            IsLineAdjustment = Convert.ToBoolean(attrSettings[1]),
                            // Check if sort order value is present, didn't exist in previous versions
                            SortOrder = attrSettings.Length > 2 ? Convert.ToInt32(attrSettings[2]) : 0
                        };
                    })
                    .OrderBy(a => a.SortOrder)
                    .ThenBy(a => a.Text)
                    .ToList();
            } else {
                return new List<ProductAttributeValue>();
            }
        }

        public static string SerializeAttributeValues(IEnumerable<ProductAttributeValue> attributeValues) {
            if (attributeValues != null) {
                return string.Join(";", attributeValues.Select(a => a.Text + "=" + a.PriceAdjustment + "," + a.IsLineAdjustment + "," + a.SortOrder));
            } else {
                return string.Empty;
            }
        }
    }
}
