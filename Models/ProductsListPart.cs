using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Newtonsoft.Json.Linq;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class ProductsListPart : ContentPart {
        //This part goes in the infoset

        private List<ShoppingCartItem> _items {
            get {
                var fromInfoset = Retrieve<string>("SerializedItems");
                if (string.IsNullOrWhiteSpace(fromInfoset)) {
                    return null;
                }
                var jsonInfoset = JObject.Parse("{'List':" + fromInfoset + "}");
                return jsonInfoset["List"].Children()
                    .Select(jt => {
                        var item = new ShoppingCartItem(
                            productId: int.Parse(jt["ProductId"].ToString()),
                            quantity: int.Parse(jt["Quantity"].ToString()),
                            attributeIdsToValues: JsonConvert.DeserializeObject<Dictionary<int, ProductAttributeValueExtended>>(jt["AttributeIdsToValues"].ToString())
                            );

                        return item;
                    })
                    .ToList();
            }
            set {
                Store<string>("SerializedItems",
                    JsonConvert.SerializeObject(value));
            }
        }

        public List<ShoppingCartItem> Items {
            get { return _items == null ? new List<ShoppingCartItem>() : _items; }
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
