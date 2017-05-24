using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListElementPartRecord : ContentPartRecord {

        public virtual string SerializedItem { get; set; }
        public virtual int WishListId { get; set; }
    }
}
