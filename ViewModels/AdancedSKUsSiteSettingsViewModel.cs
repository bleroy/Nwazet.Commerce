namespace Nwazet.Commerce.ViewModels {
    public class AdancedSKUsSiteSettingsViewModel {
        public bool RequireUniqueSKU { get; set; }
        public bool GenerateSKUAutomatically { get; set; }
        public string SKUPattern { get; set; }
        public bool AllowCustomPattern { get; set; }
    }
}
