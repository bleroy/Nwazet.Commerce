using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class ProductsListPart : ContentPart {
        //This part goes in the infoset

        private IEnumerable<ShoppingCartItem> _items {
            get {
                var fromInfoset = Retrieve<string>("SerializedItems");
                if (string.IsNullOrWhiteSpace(fromInfoset)) {
                    return null;
                }
                var jsonInfoset = JObject.Parse("{'List':" + fromInfoset + "}");
                return jsonInfoset["List"].Children()
                    .Select(jt => {
                        return new ShoppingCartItem(
                            productId: int.Parse(jt["ProductId"].ToString()),
                            quantity: int.Parse(jt["Quantity"].ToString()),
                            attributeIdsToValues: JsonConvert.DeserializeObject<Dictionary<int, ProductAttributeValueExtended>>(jt["AttributeIdsToValues"].ToString())
                            );
                    })
                    .ToList();
            }
            set {
                Store("SerializedItems", JsonConvert.SerializeObject(value));
            }
        }

        public List<ShoppingCartItem> Items {
            get { return _items != null ? _items.ToList() : new List<ShoppingCartItem>(); }
            set { _items = value; }
        }

        public string Country {
            get { return Retrieve<string>("Country"); }
            set { Store<string>("Country", value); }
        }
        public string ZipCode {
            get { return Retrieve<string>("ZipCode"); }
            set { Store<string>("ZipCode", value); }
        }

        public ShippingOption ShippingOption {
            get {
                var fromInfoset = Retrieve<string>("SerializedShippingOption");
                return string.IsNullOrWhiteSpace(fromInfoset) ? null :
                    JsonConvert.DeserializeObject<ShippingOption>(fromInfoset);
            }
            set { Store<string>("SerializedShippingOption", JsonConvert.SerializeObject(value)); }
        }

        public bool IsCart {
            get { return Retrieve<bool>("IsCart"); }
            set { Store<bool>("IsCart", value); }
        }

    }
}
