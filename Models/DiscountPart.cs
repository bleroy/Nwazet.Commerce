using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.Promotions")]
    public class DiscountPart : ContentPart<DiscountPartRecord> {
        public string Name { get { return Record.Name; } set { Record.Name = value; } }

        public decimal? DiscountPercent {
            get {
                decimal percent;
                var discount = (Retrieve(r => r.Discount) ?? "").Trim();
                if (!discount.EndsWith("%")) return null;
                if (decimal.TryParse(discount.Substring(0, discount.Length - 1), out percent)) {
                    return percent;
                }
                return null;
            }
            set { Store(r => r.Discount, value.ToString() + '%'); }
        }

        public decimal? Discount {
            get {
                decimal discount;
                var discountString = Retrieve(r => r.Discount).Trim();
                if (discountString.EndsWith("%")) return null;
                if (decimal.TryParse(discountString, out discount)) {
                    return discount;
                }
                return null;
            }
            set { Store(r => r.Discount, value.ToString()); }
        }

        public DateTime? StartDate {
            get { return Retrieve(r => r.StartDate); }
            set { Store(r => r.StartDate, value); }
        }

        public DateTime? EndDate {
            get { return Retrieve(r => r.EndDate); }
            set { Store(r => r.EndDate, value); }
        }

        public int? StartQuantity {
            get { return Retrieve(r => r.StartQuantity); }
            set { Store(r => r.StartQuantity, value); }
        }

        public int? EndQuantity {
            get { return Retrieve(r => r.EndQuantity); }
            set { Store(r => r.EndQuantity, value); }
        }
        
        public IEnumerable<string> Roles {
            get {
                var roles = Retrieve(r => r.Roles);
                if (String.IsNullOrWhiteSpace(roles)) return new string[] {};
                return roles
                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .Where(r => !String.IsNullOrEmpty(r));
            }
            set { Store(r => r.Roles, value == null ? null : String.Join(",", value)); }
        }

        public string Pattern {
            get { return Retrieve(r => r.Pattern); }
            set { Store(r => r.Pattern, value); }
        }

        public string ExclusionPattern {
            get { return Retrieve(r => r.ExclusionPattern); }
            set { Store(r => r.ExclusionPattern, value); }
        }

        public string Comment {
            get { return Retrieve(r => r.Comment); }
            set { Store(r => r.Comment, value); }
        }

        // This is only used in testing, to avoid having to stub routing logic
        public Func<IContent, string> DisplayUrlResolver { get; set; }
    }
}
