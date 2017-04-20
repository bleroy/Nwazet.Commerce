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
        private readonly ITransactionManager _transactionManager;
        public CommerceMigrations(
            IRepository<ProductPartRecord> repository,
            ITransactionManager transactionManager) {

            _repository = repository;
            _transactionManager = transactionManager;
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
            //Price, ShippingCost and DiscountPrice are now decimal rather than double
            //  SchemaBuilder.AlterTable("ProductPartRecord", table =>
            //    table.AlterColumn("Price", column =>
            //        column.WithType(DbType.Decimal)
            //    )
            //);
            //SchemaBuilder.AlterTable("ProductPartRecord", table =>
            //    table.AlterColumn("ShippingCost", column =>
            //        column.WithType(DbType.Decimal)
            //    )
            //);
            //the DiscountPrice column has a default constraint applied to it, so it cannot be dropped
            var allProducts = new List<ProductPartRecord>();
            allProducts.AddRange(_repository.Fetch(ppr => true).ToArray());

            //Drop the ProductPartRecord table and recreate it with the correct types
            SchemaBuilder.DropTable("ProductPartRecord");

            SchemaBuilder.CreateTable("ProductPartRecord", table => table
                .ContentPartRecord()
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

            SchemaBuilder.AlterTable("ProductPartRecord",
            table => table
                .CreateIndex("IDX_ProductPart_Sku", "Sku")
            );

            _repository.Flush();

            foreach (var ppr in allProducts) {
                //ProductPartRecord newProd = new ProductPartRecord() {
                //    Sku = ppr.Sku,
                //    Price = ppr.Price,
                //    DiscountPrice = ppr.DiscountPrice,
                //    IsDigital = ppr.IsDigital,
                //    ConsiderInventory = ppr.ConsiderInventory,
                //    ShippingCost = ppr.ShippingCost,
                //    Weight = ppr.Weight,
                //    Size = ppr.Size,
                //    Inventory = ppr.Inventory,
                //    OutOfStockMessage = ppr.OutOfStockMessage,
                //    AllowBackOrder = ppr.AllowBackOrder,
                //    OverrideTieredPricing = ppr.OverrideTieredPricing,
                //    PriceTiers = ppr.PriceTiers,
                //    MinimumOrderQuantity = ppr.MinimumOrderQuantity,
                //    AuthenticationRequired = ppr.AuthenticationRequired
                //};
                //newProd.ContentItemRecord = ppr.ContentItemRecord;
                _repository.Create(ppr);
            }

            return 12;
        }
    }
}
