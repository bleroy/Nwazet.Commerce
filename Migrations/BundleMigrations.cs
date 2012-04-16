using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Orchard.Indexing;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.Bundles")]
    public class BundleMigrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable(
                "BundlePartRecord",
                table => table.ContentPartRecord());

            SchemaBuilder.CreateTable(
                "BundleProductsRecord", table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<int>("BundlePartRecord_Id")
                .Column<int>("ContentItemRecord_Id"));

            SchemaBuilder.CreateForeignKey(
                "FK_BundleProductsBundle",
                "BundleProductsRecord", new[] { "BundlePartRecord_Id" },
                "BundlePartRecord", new[] {"Id"});

            SchemaBuilder.CreateForeignKey(
                "FK_BundleProductsProduct",
                "BundleProductsRecord", new[] { "ContentItemRecord_Id" },
                "Orchard.Framework", "ContentItemRecord", new[] { "Id" });

            ContentDefinitionManager.AlterPartDefinition(
                "BundlePart",builder => builder
                .Attachable());

            ContentDefinitionManager.AlterTypeDefinition(
                "Bundle", cfg => cfg
                .WithPart("Bundle")
                .WithPart("CommonPart")
                .WithPart("TitlePart")
                .WithPart("AutoroutePart", builder => builder
                    .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: '{Content.Slug}', Description: 'my-bundle'}]")
                    .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                .WithPart("BodyPart")
                .WithPart("ProductPart")
                .WithPart("BundlePart")
                .WithPart("TagsPart")
                .Creatable()
                .Indexed());

            ContentDefinitionManager.AlterPartDefinition("Bundle",
                builder => builder
                    .WithField("ProductImage", fieldBuilder => fieldBuilder.OfType("MediaPickerField").WithDisplayName("Product Image")));

            return 1;
        }

        public int UpdateFrom1() {
            SchemaBuilder.AlterTable(
                "BundleProductsRecord",
                table => table.AddColumn<int>("Quantity", column => column.WithDefault(1)));
            return 2;
        }
    }
}
