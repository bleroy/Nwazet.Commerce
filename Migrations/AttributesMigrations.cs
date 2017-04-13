using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using System;
using System.Linq;
using Orchard.Utility.Extensions;
using Nwazet.Commerce.Extensions;
using System.Collections.Generic;

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
            // Convert existing attribute data to new serlialization format (Attr1/nAttr2/n --> Attr1=0,False;Attr2=0,False)
            var existingAttributeParts = _contentManager.Query<ProductAttributePart>("ProductAttribute").List();
            foreach (var attr in existingAttributeParts) {
                attr.AttributeValuesString = ConvertSerializedAttributeValues(attr.AttributeValuesString);
            }
            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder.AlterTable("ProductAttributePartRecord", table => table
                .AddColumn<int>("SortOrder", c => c.WithDefault(0)));
            SchemaBuilder.AlterTable("ProductAttributePartRecord", table => table
                .AddColumn<string>("DisplayName"));
            return 3;
        }

        private static string ConvertSerializedAttributeValues(string values) {
            var newValues = values.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                         .Select(a => a + "=0,False");
            return string.Join(";", newValues);
        }


        public int UpdateFrom3() {
            SchemaBuilder.AlterTable("ProductAttributePartRecord", table => table
                .AddColumn<string>("TechnicalName"));

            //generate technical names for existing attributes
            var existingAttributeParts = _contentManager.Query<ProductAttributePart>("ProductAttribute").List().ToArray();
            for (int i = 0; i < existingAttributeParts.Length; i++) {
                existingAttributeParts[i].TechnicalName = 
                    AttributeNameUtilities.GenerateAttributeTechnicalName(existingAttributeParts[i], existingAttributeParts.Take(i));
            }

            return 4;
        }
        
    }
}
