using System;
using System.Linq;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    public class PriceTier {
        public int Quantity { get; set; }
        public double? Price { get; set; }
        public double? PricePercent { get; set; }

        public static IEnumerable<PriceTier> DeserializePriceTiers(string priceTiers) {
            if (priceTiers != null) {
                return priceTiers.Split(new[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Split('=')).Select(st => new PriceTier {
                        Quantity = Convert.ToInt32(st[0]),
                        Price = (!st[1].EndsWith("%") ? st[1].ToDouble() : null),
                        PricePercent = (st[1].EndsWith("%") ? st[1].Substring(0, st[1].Length - 1).ToDouble() : null)
                    })
                    .OrderBy(t => t.Quantity)
                    .ToList();
            }
            return new List<PriceTier>();
        }

        public static string SerializePriceTiers(IEnumerable<PriceTier> priceTiers) {
            if (priceTiers != null) {
                return string.Join(";", priceTiers.Select(t => t.Quantity + "=" + (t.Price != null ? t.Price.ToString() : t.PricePercent.ToString() + "%")));
            }
            return string.Empty;
        }
    }
}