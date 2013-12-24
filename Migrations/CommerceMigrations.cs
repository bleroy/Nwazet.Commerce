using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Orchard.Indexing;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.Commerce")]
    public class CommerceMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("ProductPartRecord", table => table
                .ContentPartRecord()
                .Column("Sku", DbType.String)
                .Column("Price", DbType.Double)
                .Column("ShippingCost", DbType.Single, column => column.Nullable())
                .Column("Weight", DbType.Single)
                .Column("IsDigital", DbType.Boolean, column => column.WithDefault(false))
            );

            ContentDefinitionManager.AlterPartDefinition("ProductPart",
              builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition("Product", cfg => cfg
              .WithPart("Product")
              .WithPart("CommonPart")
              .WithPart("TitlePart")
              .WithPart("AutoroutePart", builder => builder
                  .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                  .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                  .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: '{Content.Slug}', Description: 'my-product'}]")
                  .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
              .WithPart("BodyPart")
              .WithPart("ProductPart")
              .WithPart("TagsPart")
              .Creatable()
              .Indexed());

            ContentDefinitionManager.AlterPartDefinition("Product",
              builder => builder.WithField("ProductImage", fieldBuilder => fieldBuilder.OfType("MediaLibraryPickerField").WithDisplayName("Product Image")));
            return 1;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterTypeDefinition(
                "Product", cfg => cfg.Draftable());
            return 2;
        }

        public int UpdateFrom2() {
            ContentDefinitionManager.AlterTypeDefinition("ShoppingCartWidget", type => type
                .WithPart("ShoppingCartWidgetPart")
                .WithPart("CommonPart")
                .WithPart("WidgetPart")
                .WithSetting("Stereotype", "Widget")
                );

            return 3;
        }

        public int UpdateFrom3() {
            SchemaBuilder.AlterTable("ProductPartRecord", table => table
                .AddColumn<int>("Inventory", c => c.WithDefault(0)));

            return 4;
        }

        public int UpdateFrom4() {
            SchemaBuilder.AlterTable("ProductPartRecord", table => table
                .AddColumn<string>("OutOfStockMessage"));
            SchemaBuilder.AlterTable("ProductPartRecord", table => table
                .AddColumn<bool>("AllowBackOrder", c => c.WithDefault(true)));

            return 5;
        }

        public int UpdateFrom5() {
            SchemaBuilder.AlterTable("ProductPartRecord", table => table
                .AddColumn<string>("Size"));
            return 6;
        }

        public int UpdateFrom6() {
            SchemaBuilder.AlterTable("ProductPartRecord", table => table
                .AddColumn<bool>("OverrideTieredPricing", c => c.WithDefault(false)));
            SchemaBuilder.AlterTable("ProductPartRecord", table => table
                .AddColumn<string>("PriceTiers"));
            return 7;
        }
    }
}
