using System;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Models {
    public class PriceTier {
        public int Quantity { get; set; }
        public double? Price { get; set; }
        public double? PricePercent { get; set; }

        public static List<PriceTier> DeserializePriceTiers(string priceTiers) {
            if (priceTiers != null) {
                return priceTiers.Split(new[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Split('=')).Select(st => new PriceTier() {
                        Quantity = Convert.ToInt32(st[0]),
                        Price = (!st[1].EndsWith("%") ? ConvertStringToDouble(st[1]) : null),
                        PricePercent = (st[1].EndsWith("%") ? ConvertStringToDouble(st[1].Substring(0, st[1].Length - 1)) : null)
                    })
                    .OrderBy(t => t.Quantity)
                    .ToList();
            }
            else {
                return new List<PriceTier>();
            }
        }

        public static string SerializePriceTiers(List<PriceTier> priceTiers) {
            if (priceTiers != null) {
                return string.Join(";", priceTiers.Select(t => t.Quantity + "=" + (t.Price != null ? t.Price.ToString() : t.PricePercent.ToString() + "%")));
            }
            else {
                return string.Empty;
            }
        }

        public static double? ConvertStringToDouble(string ds) {
            double o;
            double? r = null;

            if (double.TryParse(ds, out o)) {
                r = o;
            };
            return r;
        }
    }
}