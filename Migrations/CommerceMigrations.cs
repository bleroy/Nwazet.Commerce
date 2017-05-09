using System.Data;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Orchard.Indexing;
using System.Linq;
using System.Collections.Generic;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.Commerce")]
    public class CommerceMigrations : DataMigrationImpl {

        private readonly IRepository<ProductPartRecord> _repository;
        IRepository<ProductPartVersionRecord> _versionRepository;
        public CommerceMigrations(
            IRepository<ProductPartRecord> repository,
            IRepository<ProductPartVersionRecord> versionRepository) {

            _repository = repository;
            _versionRepository = versionRepository;
        }

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
                builder => builder
                    .WithField("ProductImage",
                        fieldBuilder => fieldBuilder
                            .OfType("MediaLibraryPickerField")
                            .WithDisplayName("Product Image")));
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
                .AddColumn<int>("MinimumOrderQuantity"));
            SchemaBuilder.AlterTable("ProductPartRecord", table => table
                .AddColumn<bool>("AuthenticationRequired"));
            return 7;
        }

        public int UpdateFrom7() {
            SchemaBuilder.AlterTable("ProductPartRecord", table => table
                .AddColumn<bool>("OverrideTieredPricing", c => c.WithDefault(false)));
            SchemaBuilder.AlterTable("ProductPartRecord", table => table
                .AddColumn<string>("PriceTiers"));
            return 8;
        }

        public int UpdateFrom8() {
            SchemaBuilder.AlterTable("ProductPartRecord", table => table
                .AddColumn<double>("DiscountPrice", column => column
                    .NotNull().WithDefault(-1)));
            return 9;
        }

        public int UpdateFrom9() {
            SchemaBuilder.AlterTable("ProductPartRecord",
            table => table
                .CreateIndex("IDX_ProductPart_Sku", "Sku")
            );
            return 10;
        }

        public int UpdateFrom10() {
            SchemaBuilder.AlterTable("ProductPartRecord", table =>
                table.AddColumn<bool>("ConsiderInventory"));
            return 11;
        }

        public int UpdateFrom11() {
            //Versioning of ProductPart (and we take the chance to upgrade to decimal)
            SchemaBuilder.CreateTable("ProductPartVersionRecord", table => table
                .ContentPartVersionRecord()
                .Column("Sku", DbType.String)
                .Column("Price", DbType.Decimal)
                .Column("ShippingCost", DbType.Decimal, column => column.Nullable())
                .Column("Weight", DbType.Single)
                .Column("IsDigital", DbType.Boolean, column => column.WithDefault(false))
                .Column("Inventory", DbType.Int32, column => column.WithDefault(0))
                .Column("OutOfStockMessage", DbType.String)
                .Column("AllowBackOrder", DbType.Boolean, column => column.WithDefault(false))
                .Column("Size", DbType.String)
                .Column("MinimumOrderQuantity", DbType.Int32)
                .Column("AuthenticationRequired", DbType.Boolean)
                .Column("OverrideTieredPricing", DbType.Boolean, column => column.WithDefault(false))
                .Column("PriceTiers", DbType.String)
                .Column("DiscountPrice", DbType.Decimal, column => column.NotNull().WithDefault(-1))
                .Column("ConsiderInventory", DbType.Boolean)
            );

            foreach (var row in _repository.Table) {
                foreach (var version in row.ContentItemRecord.Versions) {
                    var newItem = new ProductPartVersionRecord() {
                        ContentItemRecord = row.ContentItemRecord,
                        ContentItemVersionRecord = version,
                        Sku = row.Sku,
                        Price = row.Price,
                        ShippingCost = row.ShippingCost,
                        Weight = row.Weight,
                        IsDigital = row.IsDigital,
                        Inventory = row.Inventory,
                        OutOfStockMessage = row.OutOfStockMessage,
                        AllowBackOrder = row.AllowBackOrder,
                        Size = row.Size,
                        MinimumOrderQuantity = row.MinimumOrderQuantity,
                        AuthenticationRequired = row.AuthenticationRequired,
                        OverrideTieredPricing = row.OverrideTieredPricing,
                        PriceTiers = row.PriceTiers,
                        DiscountPrice = row.DiscountPrice,
                        ConsiderInventory = row.ConsiderInventory
                    };
                    _versionRepository.Create(newItem);
                }
            }

            return 12;
        }
        
    }
}
