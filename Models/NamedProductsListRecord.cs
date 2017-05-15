using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class NamedProductsListRecord : ContentPartRecord {

        [StringLengthMax]
        public virtual string SerializedItems { get; set; } //serialized List<ShoppingCartItem>
       
        public virtual string Country { get; set; }
        public virtual string ZipCode { get; set; }
        [StringLengthMax]
        public virtual string SerializedShippingOption { get; set; }
        [StringLengthMax]
        public virtual string Event { get; set; }

        public virtual string AnonymousId { get; set; }
    }
}
