using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.Attributes")]
    public class AttributesMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("ProductAttributePartRecord", table => table
                .ContentPartRecord()
                .Column<string>("AttributeValues", col => col.Unlimited())
            );

            SchemaBuilder.CreateTable("ProductAttributesPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Attributes")
            );

            ContentDefinitionManager.AlterTypeDefinition("ProductAttribute", cfg => cfg
                .WithPart("TitlePart")
                .WithPart("ProductAttributePart"));

            ContentDefinitionManager.AlterTypeDefinition("Product", cfg => cfg
                .WithPart("ProductAttributesPart"));

            return 1;
        }
    }
}
