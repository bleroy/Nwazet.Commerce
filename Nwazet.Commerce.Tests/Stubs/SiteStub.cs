using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Settings;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;

namespace Nwazet.Commerce.Tests.Stubs {
    public class SiteStub : ISite {
        public SiteStub(bool allowOverrides, bool defineDefaults, List<PriceTier> sitePriceTiers) {
            ContentItem = new ContentItem {
                VersionRecord = new ContentItemVersionRecord {
                    ContentItemRecord = new ContentItemRecord()
                },
                ContentType = "Site"
            };
            ContentItem.Weld(new InfosetPart());

            var settings = new ProductSettingsStub(allowOverrides, defineDefaults, sitePriceTiers, ContentItem);
            ContentItem.Weld(settings);
        }

        public ContentItem ContentItem { get; private set; }
        public int MaxPageSize { get; set; }
        public int MaxPagedCount { get; set; }
        public string BaseUrl { get; set; }
        public string HomePage { get; set; }
        public bool UseCdn { get; set; }
        public int PageSize { get; set; }
        public string PageTitleSeparator { get { return ""; } }
        public string SiteCalendar { get; set; }
        public ResourceDebugMode ResourceDebugMode { get; set; }
        public string SiteCulture { get; set; }
        public string SiteName { get; set; }
        public string SiteSalt { get; set; }
        public string SiteTimeZone { get; set; }
        public string SuperUser { get; set; }
        public int Id { get; set; }
    }

    public class ProductSettingsStub : ProductSettingsPart {
        public ProductSettingsStub(bool allowOverrides, bool defineDefaults, List<PriceTier> sitePriceTiers, ContentItem contentItem) {
            ContentItem = contentItem;
            AllowProductOverrides = allowOverrides;
            DefineSiteDefaults = defineDefaults;
            PriceTiers = sitePriceTiers;
        }
    }
}
