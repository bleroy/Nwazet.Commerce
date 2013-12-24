using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using System.Linq;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.TieredPricing")]
    public class ProductSettingsPartDriver : ContentPartDriver<ProductSettingsPart> {

        public ProductSettingsPartDriver() {
        }

        protected override string Prefix {
            get { return "ProductSettings"; }
        }

        protected override DriverResult Editor(ProductSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Product_Settings",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/ProductSettings",
                    Model: new PricingSettingsViewModel() {
                        DefineSiteDefaults = part.DefineSiteDefaults,
                        AllowProductOverrides = part.AllowProductOverrides,
                        PriceTiers = part.PriceTiers
                            .Select(t => new PriceTierViewModel() {
                                Quantity = t.Quantity,
                                Price = (t.PricePercent != null ? t.PricePercent.ToString() + "%" : t.Price.ToString())
                            })
                            .ToList()
                    },
                    Prefix: Prefix)).OnGroup("Pricing");
        }

        protected override DriverResult Editor(ProductSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new PricingSettingsViewModel();
            if (updater.TryUpdateModel(model, Prefix, null, null)) {
                part.DefineSiteDefaults = model.DefineSiteDefaults;
                part.AllowProductOverrides = model.AllowProductOverrides;
                part.PriceTiers = model.PriceTiers.Select(t => new PriceTier() {
                    Quantity = t.Quantity,
                    Price = (!t.Price.EndsWith("%") ? PriceTier.ConvertStringToDouble(t.Price) : null),
                    PricePercent = (t.Price.EndsWith("%") ? PriceTier.ConvertStringToDouble(t.Price.Substring(0, t.Price.Length - 1)) : null)
                }).ToList();
            }
            return Editor(part, shapeHelper);
        }

        protected override void Importing(ProductSettingsPart part, ImportContentContext context) {
            var el = context.Data.Element(typeof(ProductSettingsPart).Name);
            if (el == null) return;
            el.With(part)
                .FromAttr(p => p.DefineSiteDefaults)
                .FromAttr(p => p.AllowProductOverrides)
                .FromAttr(p => p.PriceTiers);
        }

        protected override void Exporting(ProductSettingsPart part, ExportContentContext context) {
            context
                .Element(typeof(ProductSettingsPart).Name)
                .With(part)
                .ToAttr(p => p.DefineSiteDefaults)
                .ToAttr(p => p.AllowProductOverrides)
                .ToAttr(p => p.PriceTiers);
        }
    }
}