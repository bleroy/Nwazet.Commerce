using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Settings;
using System;
using System.Linq;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatConfigurationPartHandler : ContentHandler {

        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;
        private readonly IVatConfigurationProvider _vatConfigurationProvider;

        public VatConfigurationPartHandler(
            IRepository<VatConfigurationPartRecord> repository,
            IContentManager contentManager,
            ISiteService siteService,
            IVatConfigurationProvider vatConfigurationProvider) {

            _contentManager = contentManager;
            _siteService = siteService;
            _vatConfigurationProvider = vatConfigurationProvider;

            Filters.Add(StorageFilter.For(repository));
            Filters.Add(new ActivatingFilter<VatConfigurationSiteSettingsPart>("Site"));

            // Lazyfield setters and loaders
            OnInitializing<VatConfigurationPart>(PropertySetHandlers);
            OnLoading<VatConfigurationPart>((context, part) => LazyLoadHandlers(part));
            OnVersioning<VatConfigurationPart>((context, part, newVersionPart) => LazyLoadHandlers(newVersionPart));
            
            // manage the case where the default configuration is deleted
            OnRemoved<VatConfigurationPart>((context, part) => ResetDefaultVatConfigurationPart(part));
            OnDestroyed<VatConfigurationPart>((context, part) => ResetDefaultVatConfigurationPart(part));

            // Clean up
            OnRemoving<VatConfigurationPart>(CleanupRecords);
            OnDestroying<VatConfigurationPart>((context, part) => CleanupRecords(null, part));
        }


        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var part = context.ContentItem.As<VatConfigurationPart>();

            if (part != null) {
                context.Metadata.DisplayText = $"{part.Name} {part.TaxProductCategory}";
            }
        }
        static void PropertySetHandlers(
            InitializingContentContext context, VatConfigurationPart part) {
            
            part.HierarchiesField.Setter(value => {
                return value
                    .Where(tup => {
                        var hvcp = tup.Item1 // Item1 is the TerritoryHierarchyPart
                            .As<HierarchyVatConfigurationPart>();
                        return hvcp != null
                            && hvcp.Record.VatConfigurationIntersections
                                .Any(hvcir => hvcir.VatConfiguration == part.Record);
                    })
                    .ToList();
            });

            part.TerritoriesField.Setter(value => {
                return value
                    .Where(tup => {
                        var tvcp = tup.Item1 // Item1 is the TerritoryPart
                            .As<TerritoryVatConfigurationPart>();
                        return tvcp != null
                            && tvcp.Record.VatConfigurationIntersections
                                .Any(tvcir => tvcir.VatConfiguration == part.Record);
                    })
                    .ToList();
            });

            // call the setters in case a value had already been set
            if (part.HierarchiesField.Value != null) {
                part.HierarchiesField.Value = part.HierarchiesField.Value;
            }
        }

        void LazyLoadHandlers(VatConfigurationPart part) {
            
            part.HierarchiesField.Loader(() => {
                if (part.Record.HierarchyConfigurationIntersections != null
                    && part.Record.HierarchyConfigurationIntersections.Any()) {
                    // IEnumerable<Tuple<A, B>> pairs = listA.Zip(listB, (a, b) => Tuple.Create(a, b));
                    var listB = _contentManager
                        .GetMany<TerritoryHierarchyPart>(part.Record.HierarchyConfigurationIntersections
                            .Select(hci => hci.Hierarchy.Id),
                            VersionOptions.Latest, QueryHints.Empty);
                    return part.Record.HierarchyConfigurationIntersections
                        .Zip(listB,
                            (a, b) => Tuple.Create(b, a.Rate));
                } else {
                    return Enumerable.Empty<Tuple<TerritoryHierarchyPart, decimal>>();
                }
            });

            part.TerritoriesField.Loader(() => {
                if (part.Record.TerritoryConfigurationIntersections != null
                    && part.Record.TerritoryConfigurationIntersections.Any()) {
                    // IEnumerable<Tuple<A, B>> pairs = listA.Zip(listB, (a, b) => Tuple.Create(a, b));
                    var listB = _contentManager
                        .GetMany<TerritoryPart>(part.Record.TerritoryConfigurationIntersections
                            .Select(tvcir => tvcir.Territory.Id),
                            VersionOptions.Latest, QueryHints.Empty);
                    return part.Record.TerritoryConfigurationIntersections
                        .Zip(listB,
                            (a, b) => Tuple.Create(b, a.Rate));

                } else {
                    return Enumerable.Empty<Tuple<TerritoryPart, decimal>>();
                }
            });
        }

        void ResetDefaultVatConfigurationPart(VatConfigurationPart part) {
            // We will prevent removing the part that has the default configuration. However
            // here we still manage the case where that part is removed, in order to have a
            // further layer of data consistency. We may end up here if a delete/remove is invoked
            // without going through a permission check.
            var settings = _siteService.GetSiteSettings().As<VatConfigurationSiteSettingsPart>();
            if (settings.DefaultVatConfigurationId == part.ContentItem.Id) {
                settings.DefaultVatConfigurationId = 0;
            }
        }

        void CleanupRecords(RemoveContentContext context, VatConfigurationPart part) {
            _vatConfigurationProvider.ClearIntersectionRecords(part);
        }
    }
}
