using Nwazet.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.Taxes")]
    public class TaxDisplayTextHandler : ContentHandler {

        public TaxDisplayTextHandler() { }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var taxPart = context.ContentItem.As<ITax>();
            if (taxPart != null) {
                context.Metadata.DisplayText = $"{taxPart.Name}";
            }
        }
    }
}
