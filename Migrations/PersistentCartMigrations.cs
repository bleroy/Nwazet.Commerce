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


            ContentDefinitionManager.AlterTypeDefinition("ShoppingCart", cfg => cfg
                .WithPart("CommonPart")
                .WithPart("ProductsListPart")
            );

            return 1;
        }
    }
}
