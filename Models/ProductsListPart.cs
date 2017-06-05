using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class ProductsListPart : ContentPart {

        private const string partName = "ProductsListPart";

        //This part goes in the infoset

        private IEnumerable<ShoppingCartItem> _items {
            get {
                return RetrieveItems().ToList();
            }
            set {
                Store(value);
            }
        }

        private IEnumerable<ShoppingCartItem> RetrieveItems() {
            var partElement = GetInfosetElement();
            return partElement.Elements()
                .Where(el => el.Name.LocalName.StartsWith("ShoppingCartItem_")) //XElements corresponding to items
                .Select(el =>  //parse the XElement to an item
                    new ShoppingCartItem(
                        productId: int.Parse(el.Attribute("ProductId").Value),
                        quantity: int.Parse(el.Attribute("Quantity").Value),
                        attributeIdsToValues: el.Elements()
                            .Where(subel => subel.Name.LocalName.StartsWith("AttributeIdsToValues_")) //XElements corresponding to dictionary entries
                            .Select(subel => {
                                var id = int.Parse(subel.Attribute("Id").Value);
                                var val = new ProductAttributeValueExtended() {
                                    Value = subel.Attribute("Value")?.Value ?? "",
                                    ExtendedValue = subel.Attributes("ExtendedValue").Any() ? subel.Attribute("ExtendedValue").Value : null,
                                    ExtensionProvider = subel.Attributes("ExtensionProvider").Any() ?subel.Attribute("ExtensionProvider").Value : null
                                };
                                return new KeyValuePair<int, ProductAttributeValueExtended>(id, val);
                            }
                            )
                            .ToDictionary(pair => pair.Key, pair => pair.Value)
                        )
                );
        }

        private void Store(IEnumerable<ShoppingCartItem> items) {
            var partElement = GetInfosetElement();
            //1. Remove all items already in infoset
            var oldItemElements = partElement.Elements()
                .Where(subel => subel.Name.LocalName.StartsWith("ShoppingCartItem_"));
            foreach (var itemElement in oldItemElements.ToList()) {
                itemElement.Remove();
            }
            //2. Store new version in infoset, by adding 1 sub-element per item
            var itemsArray = items.ToArray();
            for (int i = 0; i < itemsArray.Length; i++) {
                var item = itemsArray[i];
                var itemName = "ShoppingCartItem_" + i.ToString();
                //2.1. Get the element for the item
                var itemElement = GetSubElement(partElement, itemName);
                itemElement.SetAttributeValue("ProductId", item.ProductId);
                itemElement.SetAttributeValue("Quantity", item.Quantity);
                //2.2. Remove attributes
                var oldAttrElements = itemElement.Elements()
                    .Where(subel => subel.Name.LocalName.StartsWith("AttributeIdsToValues_"));
                foreach (var attrElement in oldAttrElements.ToList()) {
                    attrElement.Remove();
                }
                //2.3. Update attributes
                var attributesArray = item.AttributeIdsToValues.ToArray();
                for (int j = 0; j < attributesArray.Length; j++) {
                    var attribute = attributesArray[j];
                    var attrName = "AttributeIdsToValues_" + j.ToString();
                    var attrElement = GetSubElement(itemElement, attrName);
                    attrElement.SetAttributeValue("Id", attribute.Key);
                    attrElement.SetAttributeValue("Value", attribute.Value.Value ?? "");
                    attrElement.SetAttributeValue("ExtendedValue", attribute.Value.ExtendedValue);
                    attrElement.SetAttributeValue("ExtensionProvider", attribute.Value.ExtensionProvider);
                }
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
                return RetrieveShippingOption();
            }
            set {
                Store(value);
            }
        }

        private void Store(ShippingOption shippingOption) {
            var partElement = GetInfosetElement();
            var shipElement = GetSubElement(partElement, "ShippingOption");
            if (shippingOption == null) {
                shipElement.RemoveAttributes();
            } else {
                shipElement.SetAttributeValue("Price", shippingOption.Price);
                shipElement.SetAttributeValue("Description", shippingOption.Description ?? "");
                shipElement.SetAttributeValue("ShippingCompany", shippingOption.ShippingCompany ?? "");
                shipElement.SetAttributeValue("IncludedShippingAreas", string.Join(",",
                    shippingOption.IncludedShippingAreas.Select(ToStringAndLength)) ?? "");
                shipElement.SetAttributeValue("ExcludedShippingAreas", string.Join(",",
                    shippingOption.ExcludedShippingAreas.Select(ToStringAndLength)) ?? "");
                shipElement.SetAttributeValue("FormValue", shippingOption.FormValue ?? "");
            }
        }
        private string ToStringAndLength(string value) {
            return string.Format(@"{{{0}}}{{{1}}}", value.Length, value);
        }

        private ShippingOption RetrieveShippingOption() {
            var partElement = GetInfosetElement();
            var shipElement = GetSubElement(partElement, "ShippingOption");
            return shipElement.Attributes().Any() ? new ShippingOption() {
                Price = decimal.Parse(shipElement.Attribute("Price").Value),
                Description = shipElement.Attribute("Description").Value,
                ShippingCompany = shipElement.Attribute("ShippingCompany").Value,
                IncludedShippingAreas = FromStringAndLength(shipElement.Attribute("IncludedShippingAreas").Value),
                ExcludedShippingAreas = FromStringAndLength(shipElement.Attribute("ExcludedShippingAreas").Value),
                FormValue = shipElement.Attribute("FormValue").Value
            } : null;
        }
        private IEnumerable<string> FromStringAndLength(string serialized) {
            var list = new List<string>();
            if (string.IsNullOrWhiteSpace(serialized)) {
                return list;
            }
            var length = 0;
            var fragments = serialized.Split(new string[] { @"}{" }, 2, StringSplitOptions.RemoveEmptyEntries);
            var num = fragments[0].TrimStart(new char[] { '{' });
            if (int.TryParse(num, out length)) {
                list.Add(fragments[1].Substring(0, length));
                if (length + 2 < fragments[1].Length) {
                    list.AddRange(FromStringAndLength(fragments[1].Substring(length + 2)));
                }
            }
            return list;
        }

        private XElement GetInfosetElement() {
            var infosetPart = this.As<InfosetPart>();
            var infoset = infosetPart.Infoset;
            return GetSubElement(infoset.Element, partName);
        }
        private XElement GetSubElement(XElement el, string name) {
            var subEl = el.Element(name);
            if (subEl == null) {
                subEl = new XElement(name);
                el.Add(subEl);
            }
            return subEl;
        }

        public bool IsCart {
            get { return Retrieve<bool>("IsCart"); }
            set { Store<bool>("IsCart", value); }
        }

    }
}
