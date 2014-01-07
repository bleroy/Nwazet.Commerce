using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using System;
using System.Linq;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.Attributes")]
    public class AttributesMigrations : DataMigrationImpl {

        private readonly IContentManager _contentManager;

        public AttributesMigrations(IContentManager contentManager) {
            _contentManager = contentManager;
        }

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

        public int UpdateFrom1() {
            // Convert existing attribute data to new serlialization format (Attr1/nAttr2/n --> Attr1=0;Attr2=0)
            var existingAttributes = _contentManager.Query("ProductAttribute").List();
            foreach (var attr in existingAttributes) {
                var attributePart = attr.As<ProductAttributePart>();
                attributePart.AttributeValuesString = ConvertSerializedAttributeValues(attributePart.AttributeValuesString);
            }
            return 2;
        }

        private static string ConvertSerializedAttributeValues(string values) {
            var newValues = values.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                         .Select(a => a + "=0");
            return string.Join(";", newValues);
        }

    }
}
