using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Usps.Shipping")]
    public class UspsMigrations : DataMigrationImpl
    {

        public int Create() {
            SchemaBuilder.CreateTable("UspsSettingsPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("UserId")
                .Column<string>("OriginZip")
                .Column<bool>("CommercialPrices")
                .Column<bool>("CommercialPlusPrices")
            );

            SchemaBuilder.CreateTable("UspsShippingMethodPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Name")
                .Column<bool>("International")
                .Column<bool>("RegisteredMail")
                .Column<bool>("Insurance")
                .Column<bool>("ReturnReceipt")
                .Column<bool>("CertificateOfMailing")
                .Column<bool>("ElectronicConfirmation")
                .Column<string>("Size")
                .Column<int>("WidthInInches")
                .Column<int>("LengthInInches")
                .Column<int>("HeightInInches")
                .Column<double>("MaximumWeightInOunces")
                .Column<double>("WeightPaddingInOunces")
                .Column<string>("ServiceNameValidationExpression")
                .Column<string>("ServiceNameExclusionExpression")
                .Column<int>("Priority")
                .Column<string>("Container")
                .Column<double>("Markup"));

            ContentDefinitionManager.AlterTypeDefinition("UspsShippingMethod", cfg => cfg
              .WithPart("UspsShippingMethodPart")
              .WithPart("TitlePart"));

            return 4;
        }

        public int UpdateFrom1() {
            SchemaBuilder.AlterTable("UspsShippingMethodPartRecord", table => table.AddColumn<string>("Size"));
            SchemaBuilder.AlterTable("UspsShippingMethodPartRecord", table => table.AddColumn<int>("WidthInInches"));
            SchemaBuilder.AlterTable("UspsShippingMethodPartRecord", table => table.AddColumn<int>("LengthInInches"));
            SchemaBuilder.AlterTable("UspsShippingMethodPartRecord", table => table.AddColumn<int>("HeightInInches"));
            SchemaBuilder.AlterTable("UspsShippingMethodPartRecord", table => table.AddColumn<double>("MaximumWeightInOunces"));
            SchemaBuilder.AlterTable("UspsShippingMethodPartRecord", table => table.AddColumn<int>("Priority"));
            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder.AlterTable("UspsShippingMethodPartRecord", table => table.AddColumn<string>("Container"));
            SchemaBuilder.AlterTable("UspsShippingMethodPartRecord", table => table.AddColumn<double>("Markup"));
            return 3;
        }

        public int UpdateFrom3() {
            SchemaBuilder.AlterTable("UspsShippingMethodPartRecord", table => table.AddColumn<double>("WeightPaddingInOunces"));
            SchemaBuilder.AlterTable("UspsShippingMethodPartRecord", table => table.AddColumn<string>("ServiceNameValidationExpression"));
            SchemaBuilder.AlterTable("UspsShippingMethodPartRecord", table => table.AddColumn<string>("ServiceNameExclusionExpression"));
            return 4;
        }
    }
}
