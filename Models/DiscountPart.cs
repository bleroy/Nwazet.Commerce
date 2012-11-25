using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Promotions")]
    public class DiscountPart : ContentPart<DiscountPartRecord> {
        public double? DiscountPercent {
            get {
                double percent;
                var discount = Record.Discount.Trim();
                if (!discount.EndsWith("%")) return null;
                if (double.TryParse(discount.Substring(0, discount.Length - 1), out percent)) {
                    return percent;
                }
                return null;
            }
            set { Record.Discount = value.ToString() + '%'; }
        }
        public double? Discount {
            get {
                double discount;
                var discountString = Record.Discount.Trim();
                if (discountString.EndsWith("%")) return null;
                if (double.TryParse(discountString, out discount)) {
                    return discount;
                }
                return null;
            }
            set { Record.Discount = value.ToString(); }
        }

        public DateTime? StartDate { get { return Record.StartDate; } set { Record.StartDate = value; } }
        public DateTime? EndDate { get { return Record.EndDate; } set { Record.EndDate = value; } }
        public int? StartQuantity { get { return Record.StartQuantity; } set { Record.StartQuantity = value; } }
        public int? EndQuantity { get { return Record.EndQuantity; } set { Record.EndQuantity = value; } }
        public IEnumerable<string> Roles {
            get {
                if (String.IsNullOrWhiteSpace(Record.Roles)) return new string[] {};
                return Record
                    .Roles
                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .Where(r => !String.IsNullOrEmpty(r));
            }
            set { Record.Roles = String.Join(",", value); }
        }
        public string Pattern { get { return Record.Pattern; } set { Record.Pattern = value; } }
        public string Comment { get { return Record.Comment; } set { Record.Comment = value; } }
    }
}
