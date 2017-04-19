using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Migrations {
    [OrchardFeature("Nwazet.AttributesLocalizationExtension")]
    class AttributesLocalizationExtensionsMigrations : DataMigrationImpl {

        public int Create() {

            ContentDefinitionManager.AlterTypeDefinition("ProductAttribute", cfg => cfg
                .WithPart("LocalizationPart"));

            return 1;
        }
    }
}
