using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListMigrations : DataMigrationImpl {
        public int Create() {

            SchemaBuilder.CreateTable("WishListListPartRecord", table => table
                .ContentPartRecord()
                .Column("SerializedIds", DbType.String)
                .Column("IsDefault", DbType.Boolean)
            );

            SchemaBuilder.CreateTable("WishListElementPartRecord", table => table
                .ContentPartRecord()
                .Column("SerializedItem", DbType.String)
                .Column("WishListId", DbType.Int32)
            );

            ContentDefinitionManager.AlterTypeDefinition("WishList", cfg => cfg
                .WithPart("CommonPart")
                .WithPart("WishListListPart")
                .WithPart("TitlePart")
            );

            ContentDefinitionManager.AlterTypeDefinition("WishListItem", cfg => cfg
                .WithPart("WishListElementPart")
            );

            ContentDefinitionManager.AlterTypeDefinition("WishListListWidget", type => type
                .WithPart("WishListListWidgetPart")
                .WithPart("CommonPart")
                .WithPart("WidgetPart")
                .WithSetting("Stereotype", "Widget")
                );

            return 1;
        }
    }
}
