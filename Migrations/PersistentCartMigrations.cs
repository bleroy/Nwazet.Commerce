using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class PersistentCartMigrations : DataMigrationImpl {

        public int Create() {

            SchemaBuilder.CreateTable("NamedProductsListRecord", table => table
                .ContentPartVersionRecord()
                .Column<string>("SerializedItems", col => col.Unlimited())
                .Column<string>("Country")
                .Column<string>("ZipCode")
                .Column<string>("SerializedShippingOption", col => col.Unlimited())
                .Column<string>("Event", col => col.Unlimited())
                .Column<string>("AnonymousId")
            );

            ContentDefinitionManager.AlterTypeDefinition("ShoppingCart", cfg => cfg
                .WithPart("CommonPart")
                .WithPart("PersistentShoppingCartPart")
            );

            return 1;
        }
    }
}
