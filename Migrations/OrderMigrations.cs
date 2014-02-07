using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.Orders")]
    public class OrderMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("OrderPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Status")
                .Column<string>("Contents", column => column.Unlimited())
                .Column<string>("Customer", column => column.Unlimited())
                .Column<string>("Activity", column => column.Unlimited())
                .Column<string>("TrackingUrl")
                .Column<string>("Password")
                .Column<bool>("IsTestOrder"));

            ContentDefinitionManager.AlterTypeDefinition("Order", type => type
                .DisplayedAs("Order")
                .WithPart("Order")
                .WithPart("OrderPart")
                .WithPart("CommonPart", p => p.WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false"))
                .WithPart("IdentityPart"));

            return 2;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterTypeDefinition("Order", type => type
                .DisplayedAs("Order")
                .WithPart("Order")
                .WithPart("OrderPart")
                .WithPart("CommonPart", p => p.WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false"))
                .WithPart("IdentityPart"));

            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder.AlterTable("OrderPartRecord", table => table
                .AddColumn<int>("UserId", column => column.WithDefault(-1)));
            return 3;
        }
    }
}
