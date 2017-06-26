using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListItemPartRecord : ContentPartRecord {

        public virtual string SerializedItem { get; set; }
        public virtual int WishListId { get; set; }
    }
}
