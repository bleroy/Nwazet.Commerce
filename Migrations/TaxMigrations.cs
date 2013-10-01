using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.Taxes")]
    public class TaxMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("StateOrCountryTaxPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("State")
                .Column<string>("Country")
                .Column<double>("Rate")
                .Column<int>("Priority")
            );

            ContentDefinitionManager.AlterTypeDefinition("StateOrCountryTax", cfg => cfg
              .WithPart("StateOrCountryTaxPart"));

            return 1;
        }
    }
}
