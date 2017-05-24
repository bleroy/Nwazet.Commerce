using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListElementPart : ContentPart<WishListElementPartRecord> {
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
    }
}
