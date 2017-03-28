using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.AdvancedSKUManagement")]
    public class AdvancedSKUManagementHandler : ContentHandler {

        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<ISKUUniquenessExceptionProvider> _SKUUniquenessExceptionProviders;

        public AdvancedSKUManagementHandler(
            IOrchardServices orchardServices,
            IContentManager contentManager,
            IEnumerable<ISKUUniquenessExceptionProvider> SKUUniquenessExceptionProviders) {

            _orchardServices = orchardServices;
            _contentManager = contentManager;

            _SKUUniquenessExceptionProviders = SKUUniquenessExceptionProviders;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override void UpdateEditorShape(UpdateEditorContext context) {
            base.UpdateEditorShape(context);
            //Apply this only for ProductParts
            if (context.ContentItem.Parts.Any(part => part is ProductPart)) {
                var productPart = context.ContentItem.As<ProductPart>();
                var settings = _orchardServices.WorkContext.CurrentSite.As<AdvancedSKUsSiteSettingPart>();
                if (settings.RequireUniqueSKU) {
                    var sameSKUProducts = _contentManager.Query<ProductPart, ProductPartRecord>(VersionOptions.Latest)
                        .Where(ppr => ppr.Id != productPart.Id && ppr.Sku == productPart.Sku)
                        .List().ToList();
                    //Handle exceptions to uniqueness of SKUs
                    if (_SKUUniquenessExceptionProviders.Any()) {
                        var exceptionIds = _SKUUniquenessExceptionProviders.SelectMany(provider => provider.GetIdsOfValidSKUDuplicates(productPart));
                        sameSKUProducts = sameSKUProducts.Where(pp => !exceptionIds.Contains(pp.Id)).ToList();
                    }
                    if (sameSKUProducts.Any()) {
                        context.Updater.AddModelError("", T("The SKU must be unique."));
                    }
                }
            }
        }
    }
}
