using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Promotions")]
    public class DiscountPart : InfosetContentPart<DiscountPartRecord> {
        public string Name { get { return Record.Name; } set { Record.Name = value; } }

        public double? DiscountPercent {
            get {
                double percent;
                var discount = (Get(r => r.Discount) ?? "").Trim();
                if (!discount.EndsWith("%")) return null;
                if (double.TryParse(discount.Substring(0, discount.Length - 1), out percent)) {
                    return percent;
                }
                return null;
            }
            set { Set(r => r.Discount, value.ToString() + '%'); }
        }

        public double? Discount {
            get {
                double discount;
                var discountString = Get(r => r.Discount).Trim();
                if (discountString.EndsWith("%")) return null;
                if (double.TryParse(discountString, out discount)) {
                    return discount;
                }
                return null;
            }
            set { Set(r => r.Discount, value.ToString()); }
        }

        public DateTime? StartDate {
            get { return Get(r => r.StartDate); }
            set { Set(r => r.StartDate, value); }
        }

        public DateTime? EndDate {
            get { return Get(r => r.EndDate); }
            set { Set(r => r.EndDate, value); }
        }

        public int? StartQuantity {
            get { return Get(r => r.StartQuantity); }
            set { Set(r => r.StartQuantity, value); }
        }

        public int? EndQuantity {
            get { return Get(r => r.EndQuantity); }
            set { Set(r => r.EndQuantity, value); }
        }
        
        public IEnumerable<string> Roles {
            get {
                var roles = Get(r => r.Roles);
                if (String.IsNullOrWhiteSpace(roles)) return new string[] {};
                return roles
                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .Where(r => !String.IsNullOrEmpty(r));
            }
            set { Set(r => r.Roles, value == null ? null : String.Join(",", value)); }
        }

        public string Pattern {
            get { return Get(r => r.Pattern); }
            set { Set(r => r.Pattern, value); }
        }

        public string Comment {
            get { return Get(r => r.Comment); }
            set { Set(r => r.Comment, value); }
        }

        // This is only used in testing, to avoid having to stub routing logic
        public Func<IContent, string> DisplayUrlResolver { get; set; }
    }
}
