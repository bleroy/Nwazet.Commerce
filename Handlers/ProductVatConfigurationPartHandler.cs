using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using System.Linq;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class ProductVatConfigurationPartHandler : ContentHandler {

        private readonly IContentManager _contentManager;
        private readonly IVatConfigurationService _vatConfigurationService;

        public ProductVatConfigurationPartHandler(
            IRepository<ProductVatConfigurationPartRecord> repository,
            IContentManager contentManager,
            IVatConfigurationService vatConfigurationService) {

            _contentManager = contentManager;
            _vatConfigurationService = vatConfigurationService;

            Filters.Add(StorageFilter.For(repository));

            //Lazyfield setters
            OnInitializing<ProductVatConfigurationPart>(PropertySetHandlers);
            OnLoading<ProductVatConfigurationPart>((context, part) => LazyLoadHandlers(part));
            OnVersioning<ProductVatConfigurationPart>((context, part, newVersionPart) => LazyLoadHandlers(newVersionPart));

        }

        protected override void Activating(ActivatingContentContext context) {
            // Attach this part wherever there is a ProductPart
            if (context.Definition.Parts.Any(pa => pa.PartDefinition.Name == "ProductPart")) {
                // the ContentItem we are activating is a product
                context.Builder.Weld<ProductVatConfigurationPart>();
            }
            base.Activating(context);
        }

        static void PropertySetHandlers(
            InitializingContentContext context, ProductVatConfigurationPart part) {
            
            part.VatConfigurationField.Setter(vatConfiguration => {
                part.Record.VatConfiguration = vatConfiguration.As<VatConfigurationPart>().Record;
                return vatConfiguration;
            });
            

            //call the setters in case a value had already been set
            if (part.VatConfigurationField.Value != null) {
                part.VatConfigurationField.Value = part.VatConfigurationField.Value;
            }
        }

        void LazyLoadHandlers(ProductVatConfigurationPart part) {
            
            part.VatConfigurationField.Loader(() => {
                if (part.Record.VatConfiguration != null) {
                    return _contentManager
                        .Get<ContentItem>(part.Record.VatConfiguration.Id,
                            VersionOptions.Latest, QueryHints.Empty);
                } else {
                    // return the default if it exists
                    if (_vatConfigurationService.GetDefaultCategoryId() > 0) {
                        return _vatConfigurationService.GetDefaultCategory().ContentItem;
                    }
                    return null;
                }

            });
        }

    }
}
