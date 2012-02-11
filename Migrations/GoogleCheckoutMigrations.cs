using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Google.Checkout")]
    public class GoogleCheckoutMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("GoogleCheckoutSettingsPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("MerchantId")
                .Column<string>("AnalyticsId")
                .Column<string>("Currency", column => column.WithDefault("USD"))
                .Column<string>("WeightUnit", column => column.WithDefault("LB"))
                .Column<bool>("UseSandbox", column => column.WithDefault(true))
            );

            return 1;
        }
    }
}
