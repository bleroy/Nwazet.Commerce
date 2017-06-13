using Orchard.ContentManagement.Records;

namespace Nwazet.Commerce.Models {
    public class WishListListPartRecord : ContentPartRecord {
        public virtual string SerializedIds { get; set; }
        public virtual bool IsDefault { get; set; }
    }
}
