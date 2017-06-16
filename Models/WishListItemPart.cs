using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using System.Collections.Generic;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListItemPart : ContentPart<WishListItemPartRecord> {
        private readonly LazyField<WishListListPart> _wishList = new LazyField<WishListListPart>();
        public LazyField<WishListListPart> WishListField { get { return _wishList; } }
        public int WishListId {
            get { return Retrieve(r => r.WishListId); }
            set { Store(r => r.WishListId, value); }
        }

        public WishListListPart WishList {
            get {
                return _wishList.Value ?? null;
            }
        }

        private ShoppingCartItem _item {
            get {
                var fromDb = Retrieve(r => r.SerializedItem);
                var json = JObject.Parse(fromDb);
                return new ShoppingCartItem(
                    productId: int.Parse(json["ProductId"].ToString()),
                    quantity: int.Parse(json["Quantity"].ToString()),
                    attributeIdsToValues: JsonConvert
                        .DeserializeObject<Dictionary<int, ProductAttributeValueExtended>>(json["AttributeIdsToValues"].ToString())
                    );
            }
            set {
                Store(r => r.SerializedItem,
              JsonConvert.SerializeObject(value));
            }
        }

        public ShoppingCartItem Item {
            get { return _item; }
            set { _item = value; }
        }

        public WishListItemPart() { }

        public WishListItemPart(int wishListId, ShoppingCartItem item) {
            WishListId = WishListId;
            _item = item;
        }
    }
}
