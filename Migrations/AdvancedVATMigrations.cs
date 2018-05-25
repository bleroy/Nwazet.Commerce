using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.AdvancedVAT")]
    public class AdvancedVATMigrations : DataMigrationImpl {

        public int Create() {

            SchemaBuilder.CreateTable("TerritoryVatConfigurationPartRecord", table => table
                .ContentPartRecord());

            SchemaBuilder.CreateTable("HierarchyVatConfigurationPartRecord", table => table
                .ContentPartRecord());

            SchemaBuilder.CreateTable("VatConfigurationPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("TaxProductCategory", col => col.Unlimited()) // uniqueness on this will have to be enforced by services
                .Column<int>("Priority")
                .Column<decimal>("DefaultRate"));

            SchemaBuilder.CreateTable("HierarchyVatConfigurationIntersectionRecord", table => table
                .Column<int>("Id", col => col.Identity().PrimaryKey())
                .Column<int>("Hierarchy_Id")
                .Column<int>("VatConfiguration_Id")
                .Column<decimal>("Rate"));
            
            SchemaBuilder.CreateForeignKey(
                "FK_HierarchyVatHierarchy",
                "HierarchyVatConfigurationIntersectionRecord", new[] { "Hierarchy_Id" },
                "HierarchyVatConfigurationPartRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey(
                "FK_HierarchyVatVat",
                "HierarchyVatConfigurationIntersectionRecord", new[] { "VatConfiguration_Id" },
                "VatConfigurationPartRecord", new[] { "Id" });

            SchemaBuilder.CreateTable("TerritoryVatConfigurationIntersectionRecord", table => table
                .Column<int>("Id", col => col.Identity().PrimaryKey())
                .Column<int>("Territory_Id")
                .Column<int>("VatConfiguration_Id")
                .Column<decimal>("Rate"));

            SchemaBuilder.CreateForeignKey(
                "FK_TerritoryVatTerritory",
                "TerritoryVatConfigurationIntersectionRecord", new[] { "Territory_Id" },
                "TerritoryVatConfigurationPartRecord", new[] { "Id" });

            SchemaBuilder.CreateForeignKey(
                "FK_TerritoryVatVat",
                "TerritoryVatConfigurationIntersectionRecord", new[] { "VatConfiguration_Id" },
                "VatConfigurationPartRecord", new[] { "Id" });

            ContentDefinitionManager.AlterTypeDefinition("VATConfiguration", cfg => cfg
                .WithPart("VatConfigurationPart")
                .DisplayedAs("VAT Category Configuration"));

            SchemaBuilder.CreateTable("ProductVatConfigurationPartRecord", table=>table
                .ContentPartRecord()
                .Column<int>("VatConfiguration_Id"));

            SchemaBuilder.CreateTable("OrderVatRecord", table => table
                .Column<int>("Id", col => col.Identity().PrimaryKey())
                .Column<int>("OrderPartRecord_Id")
                .Column<string>("Information", col => col.Unlimited()));

            return 1;
        }

    }
}
