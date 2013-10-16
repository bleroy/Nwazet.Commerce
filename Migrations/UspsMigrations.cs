using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Usps.Shipping")]
    public class UspsMigrations : DataMigrationImpl
    {

        public int Create() {
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

            return 1;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterTypeDefinition("UspsShippingMethod", cfg => cfg
              .WithPart("UspsShippingMethodPart")
              .WithPart("TitlePart"));
            return 4;
        }
    }
}
