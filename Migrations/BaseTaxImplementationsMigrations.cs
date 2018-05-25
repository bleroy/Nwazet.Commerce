using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using System;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.BaseTaxImplementations")]
    public class BaseTaxImplementationsMigrations : DataMigrationImpl {

        public int Create() {
            // we could be calling this because we enabled the feature in the migrations for 
            // Nwazet.Taxes. In that case, we do not have to actually create the tables, because
            // those were handled there in the past.

            try {
                // If the table exists, this next method call succeeds. Otherwise, it throws an exception.
                SchemaBuilder.ExecuteSql("SELECT * FROM Nwazet_Commerce_StateOrCountryTaxPartRecord");
            } catch (Exception) {
                // Table does not exist
                SchemaBuilder.CreateTable("StateOrCountryTaxPartRecord", table => table
                    .ContentPartRecord()
                    .Column<string>("State")
                    .Column<string>("Country")
                    .Column<decimal>("Rate")
                    .Column<int>("Priority")
                );
            }

            ContentDefinitionManager.AlterTypeDefinition("StateOrCountryTax", cfg => cfg
              .WithPart("StateOrCountryTaxPart"));

            ContentDefinitionManager.AlterTypeDefinition("ZipCodeTax", cfg => cfg
                .WithPart("ZipCodeTaxPart"));

            return 1;
        }

    }
}
