using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListListPart : ContentPart<WishListListPartRecord> {
        private readonly LazyField<IEnumerable<ContentItem>> _wishListItems = new LazyField<IEnumerable<ContentItem>>();
        public LazyField<IEnumerable<ContentItem>> WishListItemsField { get { return _wishListItems; } }

        public int[] Ids {
            get {
                return WishListItems
                    .Select(it => it.Id)
                    .ToArray();
            }
        }

        public IEnumerable<ContentItem> WishListItems {
            get {
                return _wishListItems.Value ?? Enumerable.Empty<ContentItem>();
            }
        }

        public bool IsDefault {
            get { return Retrieve(r => r.IsDefault); }
            set { Store(r => r.IsDefault, value); }
        }
        
    }
}
