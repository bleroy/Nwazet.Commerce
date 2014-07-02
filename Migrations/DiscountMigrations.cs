using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.Promotions")]
    public class DiscountMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("DiscountPartRecord", table => table
                .ContentPartRecord()
                .Column("Name", DbType.String)
                .Column("Discount", DbType.String)
                .Column("StartDate", DbType.DateTime, column => column.Nullable())
                .Column("EndDate", DbType.DateTime, column => column.Nullable())
                .Column("StartQuantity", DbType.Int32, column => column.Nullable())
                .Column("EndQuantity", DbType.Int32, column => column.Nullable())
                .Column("Roles", DbType.String)
                .Column("Pattern", DbType.String)
                .Column("ExclusionPattern", DbType.String)
                .Column("Comment", DbType.String)
            );

            ContentDefinitionManager.AlterTypeDefinition("Discount", cfg => cfg
              .WithPart("DiscountPart")
              .WithPart("TitlePart"));

            return 2;
        }

        public int UpdateFrom1() {
            SchemaBuilder.AlterTable("DiscountPartRecord", table => table
                .AddColumn<string>("ExclusionPattern"));

            return 2;
        }
    }
}
