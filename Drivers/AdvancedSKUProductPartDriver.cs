using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.AdvancedSKUManagement")]
    public class AdvancedSKUProductPartDriver : ContentPartDriver<ProductPart> {

        private readonly ISKUGenerationServices _SKUGenerationServices;

        public AdvancedSKUProductPartDriver(ISKUGenerationServices SKUGenerationServices) {

            _SKUGenerationServices = SKUGenerationServices;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        protected override string Prefix
        {
            get { return "NwazetCommerceProduct"; }
        }

        protected override DriverResult Editor(ProductPart part, dynamic shapeHelper) {

            var settings = _SKUGenerationServices.GetSettings();
            var model = new AdvancedSKUProductEditorViewModel() {
                Product = part,
                CurrentSku = part.Sku,
                Settings = settings,
                SkuPattern = string.IsNullOrWhiteSpace(settings.SKUPattern) ? _SKUGenerationServices.DefaultSkuPattern : settings.SKUPattern
            };
            if (string.IsNullOrWhiteSpace(part.Sku)) {
                part.Sku = _SKUGenerationServices.DefaultSkuPattern;
            }

            return ContentShape("Part_Product_SKUEdit",
                () => shapeHelper.EditorTemplate(
                TemplateName: "Parts/AdvancedSKUProduct",
                Model: model, 
                Prefix: Prefix));
        }

        protected override DriverResult Editor(ProductPart part, IUpdateModel updater, dynamic shapeHelper) {
            //The part has already been updated by the default driver here
            var model = new AdvancedSKUProductEditorViewModel();
            //if (updater.TryUpdateModel(model, Prefix, null, null)) {
            //    part.Sku = model.CurrentSku;
            //}
            return Editor(part, shapeHelper);
        }
    }
}
