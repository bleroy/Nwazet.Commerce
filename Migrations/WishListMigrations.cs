using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListMigrations : DataMigrationImpl {
        public int Create() {

            ContentDefinitionManager.AlterTypeDefinition("WishList", cfg => cfg
                .WithPart("CommonPart")
                .WithPart("WishListListPart")
                .WithPart("TitlePart")
            );

            ContentDefinitionManager.AlterTypeDefinition("WishListItem", cfg => cfg
                .WithPart("WishListElementPart")
            );

            return 1;
        }
    }
}
