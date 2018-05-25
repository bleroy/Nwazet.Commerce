using Nwazet.Commerce.Models;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Notify;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class VatConfigurationPartDriver : ContentPartDriver<VatConfigurationPart> {

        private readonly IContentManager _contentManager;
        private readonly ITerritoriesService _territoriesService;
        private readonly IVatConfigurationService _vatConfigurationService;
        private readonly IAuthorizer _authorizer;
        private readonly INotifier _notifier;

        public VatConfigurationPartDriver(
            IContentManager contentManager,
            ITerritoriesService territoriesService,
            IVatConfigurationService vatConfigurationService,
            IAuthorizer authorizer,
            INotifier notifier) {

            _contentManager = contentManager;
            _territoriesService = territoriesService;
            _vatConfigurationService = vatConfigurationService;
            _authorizer = authorizer;
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }

        public Localizer T;

        protected override string Prefix {
            get { return "VatConfigurationPart"; }
        }

        protected override DriverResult Editor(VatConfigurationPart part, dynamic shapeHelper) {
            if (!_authorizer.Authorize(CommercePermissions.ManageTaxes)) {
                _notifier.Warning(T("Changes to the VAT configuration will not be saved because you don't have the correct permissions."));
            }
            var model = CreateVM(part);
            var configIssues = CheckIntersectionsBetweenHierarchies(part);
            if (configIssues.Any()) {
                var sb = new StringBuilder();
                sb.AppendLine(T("There are issues with the hierarchies configured for this VAT:").Text);
                foreach (var issue in configIssues) {
                    sb.AppendLine(T("\t{0} and {1} intersect on {2}", issue.Hierarchy1, issue.Hierarchy2, issue.Territory).Text);
                }
                _notifier.Warning(T("{0}", sb.ToString()));
            }
            return ContentShape("Parts_VatConfiguration_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/VatConfiguration",
                    Model: model,
                    Prefix: Prefix
                    ));
        }

        protected override DriverResult Editor(VatConfigurationPart part, IUpdateModel updater, dynamic shapeHelper) {
            if (_authorizer.Authorize(CommercePermissions.ManageTaxes)) {
                var model = new VatConfigurationPartViewModel();
                if (updater.TryUpdateModel(model, Prefix, null, null)) {
                    part.Priority = model.Priority;
                    part.TaxProductCategory = model.TaxProductCategory;
                    // Check default category flag
                    if (model.IsDefaultCategory) {
                        _vatConfigurationService.SetDefaultCategory(part);
                    }
                    part.DefaultRate = model.DefaultRate;
                }
            }
            return Editor(part, shapeHelper);
        }

        private VatConfigurationPartViewModel CreateVM(VatConfigurationPart part) {
            // If no default VatConfigurationPart exists the GetDefaultCategoryId method returns 0,
            // as is defined in the interface. This way, the next new VatConfigurationPart that is created
            // will automatically becom the new default.
            var partIsDefault = part.ContentItem.Id == _vatConfigurationService.GetDefaultCategoryId();
            return new VatConfigurationPartViewModel {
                TaxProductCategory = part.TaxProductCategory,
                IsDefaultCategory = partIsDefault,
                DefaultRate = part.DefaultRate,
                Priority = part.Priority,
                Part = part,
                ItemizedSummary = BuildSummary(part)
            };
        }

        private List<VatConfigurationHierarchySummaryViewModel> BuildSummary(VatConfigurationPart part) {

            return part.Hierarchies == null
                ? new List<VatConfigurationHierarchySummaryViewModel>()
                : part.Hierarchies
                    .Select(tup => new VatConfigurationHierarchySummaryViewModel {
                        Name = _contentManager.GetItemMetadata(tup.Item1).DisplayText,
                        Item = tup.Item1.ContentItem,
                        Rate = tup.Item2,
                        SubRegions = part.Territories == null
                            ? new List<VatConfigurationTerritorySummaryViewModel>()
                            :part.Territories
                                .Where(tpd => tpd.Item1.HierarchyPart.Record == tup.Item1.Record)
                                .Select(tpd => new VatConfigurationTerritorySummaryViewModel {
                                    Name = _contentManager.GetItemMetadata(tpd.Item1).DisplayText,
                                    Item = tpd.Item1.ContentItem,
                                    Rate = tpd.Item2
                                })
                                .ToList()
                    })
                    .ToList();
        }

        private IEnumerable<HierarchyIntersection> CheckIntersectionsBetweenHierarchies(VatConfigurationPart part) {
            
            if (part.Hierarchies != null
                && part.Hierarchies.Count() > 1) {
                // get the TerritoryHierarchyParts
                var hierarchies = part.Hierarchies.Select(tup => tup.Item1).ToArray();
                for (int i = 0; i < hierarchies.Length-1; i++) {
                    var source = hierarchies[i].Territories.Select(ci => ci.As<TerritoryPart>());
                    var sourceString = _contentManager.GetItemMetadata(hierarchies[i]).DisplayText;
                    for (int j = i+1; j < hierarchies.Length; j++) {
                        var other = hierarchies[j].Territories.Select(ci => ci.As<TerritoryPart>());
                        var otherString = _contentManager.GetItemMetadata(hierarchies[j]).DisplayText;
                        var intersection = source.Intersect(other, new TerritoryPart.TerritoryPartComparer());
                        if (intersection.Any()) {
                            foreach (var territory in intersection) {

                                yield return new HierarchyIntersection {
                                    Hierarchy1 = sourceString,
                                    Hierarchy2 = otherString,
                                    Territory = territory.Record.TerritoryInternalRecord.Name
                                };
                            }
                        }
                    }
                }
            }

            yield break;
        }

        class HierarchyIntersection {
            public string Hierarchy1 { get; set; }
            public string Hierarchy2 { get; set; }
            public string Territory { get; set; }
        }

    }
}
