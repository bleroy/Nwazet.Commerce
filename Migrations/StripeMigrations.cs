using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Stripe")]
    public class StripeMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("StripeSettingsPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("PublishableKey", column => column.WithLength(32))
                .Column<string>("SecretKey", column => column.WithLength(32))
                .Column<string>("Currency", column => column.WithLength(3))
            );

            return 1;
        }
    }
}
