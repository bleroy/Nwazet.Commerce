using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Notify;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.AdvancedSKUManagement")]
    public class AdvancedSKUManagementHandler : ContentHandler {

        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<ISKUUniquenessHelper> _SKUUniquenessHelpers;
        private readonly Lazy<ISKUGenerationServices> _SKUGenerationServices;

        public AdvancedSKUManagementHandler(
            IOrchardServices orchardServices,
            IContentManager contentManager,
            IEnumerable<ISKUUniquenessHelper> SKUUniquenessHelpers,
            Lazy<ISKUGenerationServices> SKUGenerationServices) {

            _orchardServices = orchardServices;
            _contentManager = contentManager;

            _SKUUniquenessHelpers = SKUUniquenessHelpers;

            _SKUGenerationServices = SKUGenerationServices;

            T = NullLocalizer.Instance;

            OnUpdated<ProductPart>((ctx, part) => CreateSku(part));
            OnCreated<ProductPart>((ctx, part) => {
                if (part.ContentItem.VersionRecord == null) {
                    CreateSku(part);
                }
            });
        }

        public Localizer T { get; set; }

        protected override void UpdateEditorShape(UpdateEditorContext context) {
            base.UpdateEditorShape(context);
            //Apply this only for ProductParts
            if (context.ContentItem.Parts.Any(part => part is ProductPart)) {
                var productPart = context.ContentItem.As<ProductPart>();
                var settings = _orchardServices.WorkContext.CurrentSite.As<AdvancedSKUsSiteSettingPart>();
                if (settings.RequireUniqueSKU) {
                    var sameSKUProductIds = _contentManager.Query<ProductPart, ProductPartVersionRecord>(VersionOptions.Latest)
                        .Where(ppr => ppr.Id != productPart.Record.Id && ppr.Sku == productPart.Sku)
                        .List().Select(pp => pp.Id);
                    //Handle exceptions to uniqueness of SKUs
                    if (_SKUUniquenessHelpers.Any()) {
                        var exceptionIds = _SKUUniquenessHelpers.SelectMany(provider => provider.GetIdsOfValidSKUDuplicates(productPart));
                        sameSKUProductIds = sameSKUProductIds.Where(ppid => !exceptionIds.Contains(ppid));
                    }
                    if (sameSKUProductIds.Any()) {
                        context.Updater.AddModelError("", T("The SKU must be unique."));
                    }
                }
            }
        }

        private void CreateSku(ProductPart part) {
            if (_SKUGenerationServices.Value.GetSettings().GenerateSKUAutomatically) {
                ProcessSku(part);
            }
        }

        private void ProcessSku (ProductPart part) {
            if (string.IsNullOrWhiteSpace(part.Sku) ||
                _SKUGenerationServices.Value.GetSettings().AllowCustomPattern) { //AllowCustomPattern is handled inside GenerateSku
                //generate a new sku
                part.Sku = _SKUGenerationServices.Value.GenerateSku(part);
            }

            //if the SKU is empty, generate a new one
            if (string.IsNullOrWhiteSpace(part.Sku)) {
                _SKUGenerationServices.Value.ProcessSku(part);
                _orchardServices.Notifier.Information(T("A new SKU has been generated: \"{0}\"", part.Sku));
                return;
            }

            //check for SKU conflicts
            var previous = part.Sku;
            if (!_SKUGenerationServices.Value.ProcessSku(part)) {
                _orchardServices.Notifier.Warning(
                    T("Conflict between SKUs. \"{0}\" is already set for a previously created {2} so now we set it to \"{1}\"",
                        previous, part.Sku, part.ContentItem.ContentType));
            }
        }
        
    }
}
