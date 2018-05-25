using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatConfigurationProvider : ITaxProvider, IVatConfigurationProvider {

        private readonly IContentManager _contentManager;
        private readonly IRepository<HierarchyVatConfigurationIntersectionRecord> _hierarchyVatConfigurations;
        private readonly IRepository<TerritoryVatConfigurationIntersectionRecord> _territoryVatConfigurations;
        private readonly IWorkContextAccessor _workContextAccessor;

        private Localizer T { get; set; }

        public VatConfigurationProvider(
            IContentManager contentManager,
            IRepository<HierarchyVatConfigurationIntersectionRecord> hierarchyVatConfigurations,
            IRepository<TerritoryVatConfigurationIntersectionRecord> territoryVatConfigurations,
            IWorkContextAccessor workContextAccessor) {

            _contentManager = contentManager;
            _hierarchyVatConfigurations = hierarchyVatConfigurations;
            _territoryVatConfigurations = territoryVatConfigurations;
            _workContextAccessor = workContextAccessor;

            T = NullLocalizer.Instance;
        }

        public string ContentTypeName {
            get { return "VATConfiguration"; }
        }

        public string Name {
            get { return T("VAT Category Configuration").Text; }
        }

        public IEnumerable<ITax> GetTaxes() {
            // We cheat a bit here. We return only one object, because when computing things for VAT
            // we are going to use the objects referenced by the products anyway.
            // ERROR: returning only one item is wrong, because the TaxAdminController needs to show them all
            return _contentManager
                .Query<VatConfigurationPart, VatConfigurationPartRecord>()
                .ForVersion(VersionOptions.Published)
                .List();
        }

        public IEnumerable<VatConfigurationPart> GetVatConfigurations() {
            return _contentManager
                .Query<VatConfigurationPart, VatConfigurationPartRecord>()
                .ForVersion(VersionOptions.Published)
                .List();
        }

        public IEnumerable<VatConfigurationPart> GetVatConfigurations(VersionOptions versionOptions) {
            return _contentManager
                .Query<VatConfigurationPart, VatConfigurationPartRecord>()
                .ForVersion(versionOptions)
                .List();
        }

        /// <summary>
        /// We lock on string.Intern(LockString) to make sure we do not mess things up when updating stuff
        /// in the IRepositories. The risk is to have races on some data that would result in invalid configurations
        /// if a hierarchy and one of its territories are being updated at the same time.
        /// TODO: Upgrade to use ILockingProvider when it becomes available from the dev branch.
        /// </summary>
        private string LockString {
            get {
                return string.Join(".",
                    _workContextAccessor.GetContext()?.CurrentSite?.BaseUrl ?? "",
                    _workContextAccessor.GetContext()?.CurrentSite?.SiteName ?? "",
                    "VatConfigurationProvider.RepositoriesLock");
            }
        }

        public void UpdateConfiguration(
            HierarchyVatConfigurationPart part, HierarchyVatConfigurationPartViewModel model) {
            lock (string.Intern(LockString)) {
                // fetch all configurations for the part here to avoid fetching them one by one as needed
                var allConfigurations = _hierarchyVatConfigurations
                    .Fetch(r => r.Hierarchy == part.Record);

                foreach (var detailVM in model.AllVatConfigurations) {
                    var oldCfg = part.VatConfigurations
                        ?.FirstOrDefault(tup => tup.Item1.Record.Id == detailVM.VatConfigurationPartId);
                    if (oldCfg == null) {
                        if (detailVM.IsSelected) {
                            // we added a new VAT category configuration to this hiehrarchy
                            _hierarchyVatConfigurations.Create(
                                new HierarchyVatConfigurationIntersectionRecord {
                                    Hierarchy = part.Record,
                                    VatConfiguration = _contentManager
                                        .Get<VatConfigurationPart>(detailVM.VatConfigurationPartId)
                                        .Record,
                                    Rate = detailVM.Rate.HasValue ? detailVM.Rate.Value : 0
                                });
                        }
                    } else {
                        var intersection = allConfigurations
                            .FirstOrDefault(r => r.VatConfiguration == oldCfg.Item1.Record);
                        if (detailVM.IsSelected) {
                            // update Rate if different
                            if (oldCfg.Item2 != detailVM.Rate) {
                                intersection.Rate = detailVM.Rate.HasValue ? detailVM.Rate.Value : 0;
                                _hierarchyVatConfigurations.Update(intersection);
                            }
                        } else {
                            // we removed a VAT category configuration
                            _hierarchyVatConfigurations.Delete(intersection);
                            // We need to go through all territories and delete the configurations there as well
                            var territories = part
                                .As<TerritoryHierarchyPart>() // get the hierarchy
                                ?.Territories // all its Territory ContentItems
                                ?.Select(ci => ci.As<TerritoryVatConfigurationPart>()) // where there is an attached part to configure VAT
                                ?.Where(tvcp => tvcp != null); // and such part is not null

                            if (territories != null && territories.Any()) {
                                var territoryIds = territories.Select(tvcp => tvcp.Record.Id);
                                var toDelete = _territoryVatConfigurations
                                    .Fetch(ir => territoryIds.Contains(ir.Territory.Id)
                                        && ir.VatConfiguration == oldCfg.Item1.Record);
                                foreach (var entity in toDelete) {
                                    _territoryVatConfigurations.Delete(entity);
                                }
                            }
                        }
                    }
                }
            }

        }

        public void UpdateConfiguration(
            TerritoryVatConfigurationPart part, TerritoryVatConfigurationPartViewModel model) {
            lock (string.Intern(LockString)) {
                // fetch all configurations fro the part here to avoid fetching them one by one as needed
                var allConfigurations = _territoryVatConfigurations
                    .Fetch(r => r.Territory == part.Record);

                // When adding or updating configurations, we should double check that it is still available for the
                // hierarchy
                var hierarchy = part.As<TerritoryPart>()?.HierarchyPart ??
                    part.As<TerritoryPart>()?.CreationHierarchy;
                var hierarchyConfigurations = hierarchy
                    ?.As<HierarchyVatConfigurationPart>()
                    ?.VatConfigurations
                    ?.Select(tup => tup.Item1);
                if (hierarchyConfigurations == null || !hierarchyConfigurations.Any()) {
                    // no vat configuration allowed
                    foreach (var entity in allConfigurations) {
                        _territoryVatConfigurations.Delete(entity);
                    }
                } else {
                    foreach (var detailVM in model.AllVatConfigurations) {
                        var oldCfg = part.VatConfigurations
                            ?.FirstOrDefault(tup => tup.Item1.Record.Id == detailVM.VatConfigurationPartId);
                        var configurationAllowed = hierarchyConfigurations.Any(vcp => vcp.Record.Id == detailVM.VatConfigurationPartId);
                        if (hierarchyConfigurations.Any(vcp => vcp.Record.Id == detailVM.VatConfigurationPartId)) {
                            // the VAT category configuration is allowed
                            if (oldCfg == null) {
                                if (detailVM.IsSelected) {
                                    // we added a new VAT category configuration to this hierarchy
                                    _territoryVatConfigurations.Create(
                                        new TerritoryVatConfigurationIntersectionRecord {
                                            Territory = part.Record,
                                            VatConfiguration = _contentManager
                                                .Get<VatConfigurationPart>(detailVM.VatConfigurationPartId)
                                                .Record,
                                            Rate = detailVM.Rate.HasValue ? detailVM.Rate.Value : 0
                                        });
                                }
                            } else {
                                var intersection = allConfigurations
                                    .FirstOrDefault(r => r.VatConfiguration == oldCfg.Item1.Record);
                                if (detailVM.IsSelected) {
                                    // Update rate if it has changed
                                    if (oldCfg.Item2 != detailVM.Rate) {
                                        intersection.Rate = detailVM.Rate.HasValue ? detailVM.Rate.Value : 0;
                                        _territoryVatConfigurations.Update(intersection);
                                    }
                                } else {
                                    // we removed a VAT category configuration
                                    _territoryVatConfigurations.Delete(intersection);
                                }
                            }
                        } else {
                            // since the configuration is not allowed at the hierarchy level, we only need to eventually
                            // remove it
                            if (oldCfg != null) {
                                var intersection = allConfigurations
                                   .FirstOrDefault(r => r.VatConfiguration == oldCfg.Item1.Record);
                                _territoryVatConfigurations.Delete(intersection);
                            }
                        }
                        
                    }
                }
            }
        }

        public void ClearIntersectionRecords(VatConfigurationPart part) {
            lock (string.Intern(LockString)) {
                var allHierarchyConfigurations = _hierarchyVatConfigurations
                    .Fetch(r => r.VatConfiguration == part.Record);
                foreach (var entity in allHierarchyConfigurations) {
                    _hierarchyVatConfigurations.Delete(entity);
                }
                var allTerritoryConfigurations = _territoryVatConfigurations
                    .Fetch(r => r.VatConfiguration == part.Record);
                foreach (var entity in allTerritoryConfigurations) {
                    _territoryVatConfigurations.Delete(entity);
                }
            }
        }

        public void ClearIntersectionRecords(HierarchyVatConfigurationPart part) {
            lock (string.Intern(LockString)) {
                var allConfigurations = _hierarchyVatConfigurations
                    .Fetch(r => r.Hierarchy == part.Record);
                foreach (var entity in allConfigurations) {
                    _hierarchyVatConfigurations.Delete(entity);
                }
            }
        }

        public void ClearIntersectionRecords(TerritoryVatConfigurationPart part) {
            lock (string.Intern(LockString)) {
                var allConfigurations = _territoryVatConfigurations
                    .Fetch(r => r.Territory == part.Record);
                foreach (var entity in allConfigurations) {
                    _territoryVatConfigurations.Delete(entity);
                }
            }
        }
    }
}
