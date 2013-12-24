using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Services;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.TieredPricing")]
    public class TieredPriceProvider : ITieredPriceProvider {
        private readonly IWorkContextAccessor _wca;

        public TieredPriceProvider(IWorkContextAccessor wca) {
            _wca = wca;
        }

        public ShoppingCartQuantityProduct GetTieredPrice(ShoppingCartQuantityProduct quantityProduct) {
            var priceTiers = GetPriceTiers(quantityProduct.Product);
            var priceTier = priceTiers != null ? priceTiers.Where(t => t.Quantity <= quantityProduct.Quantity).OrderByDescending(t => t.Quantity).Take(1).SingleOrDefault() : null;
            if (priceTier != null) {
                quantityProduct.Price = (double)priceTier.Price;
            }
            return quantityProduct;
        }

        public IEnumerable<PriceTier> GetPriceTiers(ProductPart product) {
            var productSettings = _wca.GetContext().CurrentSite.As<ProductSettingsPart>();
            IEnumerable<PriceTier> priceTiers = null;

            if (productSettings.AllowProductOverrides && product.OverrideTieredPricing) {
                priceTiers = product.PriceTiers;
            }
            else if (productSettings.DefineSiteDefaults && (!productSettings.AllowProductOverrides || !product.OverrideTieredPricing)) {
                priceTiers = productSettings.PriceTiers;
            }

            if (priceTiers != null) {
                foreach (var tier in priceTiers) {
                    if (tier.Price == null && tier.PricePercent != null) {
                        tier.Price = product.Price * (double)tier.PricePercent / 100;
                    }
                }
                return priceTiers.OrderBy(t => t.Quantity);
            }

            return priceTiers;
        }
    }
}
