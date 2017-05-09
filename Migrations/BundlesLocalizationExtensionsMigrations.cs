using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.BundlesLocalizationExtension")]
    public class BundlesLocalizationExtensionsMigrations : DataMigrationImpl {

        public int Create() {

            ContentDefinitionManager.AlterTypeDefinition("Product", cfg => cfg
                .WithPart("LocalizationPart"));

            return 1;
        }
    }
}
