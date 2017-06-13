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
        //Name strings for XElements
        private const string partName = "ProductsListPart";

        //This part goes in the infoset

        private IEnumerable<ShoppingCartItem> _items {
            get {
                return RetrieveItems();
            }
            set {
                Store(value);
            }
        }

        private IEnumerable<ShoppingCartItem> RetrieveItems() {
            var partElement = GetInfosetElement();
            var cartElement = partElement.Element("ShoppingCartItems");
            if (cartElement == null) {
                return null;
            }
            return cartElement.Elements("ShoppingCartItem")
                .Select(itemElement =>
                    new ShoppingCartItem(
                        productId: (int)(itemElement.Attribute("ProductId")),
                        quantity: (int)(itemElement.Attribute("Quantity")),
                        attributeIdsToValues: itemElement.Element("AttributeIdsToValues") == null ? null :
                            itemElement.Element("AttributeIdsToValues")
                            .Elements("AttributeInfo")
                            .Select(attrElement =>
                                new KeyValuePair<int, ProductAttributeValueExtended>(
                                    (int)(attrElement.Attribute("Id")),
                                    new ProductAttributeValueExtended() {
                                        Value = attrElement.Attribute("Value")?.Value ?? "",
                                        ExtendedValue = attrElement.Attributes("ExtendedValue").Any() ? attrElement.Attribute("ExtendedValue").Value : null,
                                        ExtensionProvider = attrElement.Attributes("ExtensionProvider").Any() ? attrElement.Attribute("ExtensionProvider").Value : null
                                    }
                                    )
                            )
                            .ToDictionary(pair => pair.Key, pair => pair.Value)
                        )
                );
        }

        private void Store(IEnumerable<ShoppingCartItem> items) {
            var partElement = GetInfosetElement();
            //1. Remove all items already in infoset
            var oldItemsElement = partElement.Element("ShoppingCartItems");
            if (oldItemsElement != null) {
                oldItemsElement.Remove();
            }
            //2. Store new version in infoset, by adding 1 sub-element per item
            var itemsElement = GetSubElement(partElement, "ShoppingCartItems");
            itemsElement.Add(items.Select(sci => {
                var itemEl = new XElement("ShoppingCartItem");
                itemEl.SetAttributeValue("ProductId", sci.ProductId);
                itemEl.SetAttributeValue("Quantity", sci.Quantity);
                if (sci.AttributeIdsToValues != null && sci.AttributeIdsToValues.Any()) {
                    var attrElement = GetSubElement(itemEl, "AttributeIdsToValues");
                    attrElement.Add(sci.AttributeIdsToValues.Select(kvp => {
                        var atEl = new XElement("AttributeInfo");
                        atEl.SetAttributeValue("Id", kvp.Key);
                        atEl.SetAttributeValue("Value", kvp.Value.Value ?? "");
                        atEl.SetAttributeValue("ExtendedValue", kvp.Value.ExtendedValue);
                        atEl.SetAttributeValue("ExtensionProvider", kvp.Value.ExtensionProvider);
                        return atEl;
                    }));
                }
                return itemEl;
            }));
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
                shipElement.RemoveAll();
                shipElement.Remove();
            } else {
                shipElement.SetAttributeValue("Price", shippingOption.Price);
                shipElement.SetAttributeValue("Description", shippingOption.Description ?? "");
                shipElement.SetAttributeValue("ShippingCompany", shippingOption.ShippingCompany ?? "");
                if (shippingOption.IncludedShippingAreas.Any()) {
                    var includedAreasElement = GetSubElement(shipElement, "IncludedShippingAreas");
                    foreach (var area in shippingOption.IncludedShippingAreas) {
                        includedAreasElement.Add(new XElement("Area", area));
                    }
                }
                if (shippingOption.ExcludedShippingAreas.Any()) {
                    var excludedAreasElement = GetSubElement(shipElement, "ExcludedShippingAreas");
                    foreach (var area in shippingOption.ExcludedShippingAreas) {
                        excludedAreasElement.Add(new XElement("Area", area));
                    }
                }
                shipElement.SetAttributeValue("FormValue", shippingOption.FormValue ?? "");
            }
        }

        private ShippingOption RetrieveShippingOption() {
            var partElement = GetInfosetElement();
            var shipElement = GetSubElement(partElement, "ShippingOption");
            return shipElement.Attributes().Any() ? new ShippingOption() {
                Price = (decimal)(shipElement.Attribute("Price")),
                Description = shipElement.Attribute("Description").Value,
                ShippingCompany = shipElement.Attribute("ShippingCompany").Value,
                IncludedShippingAreas = shipElement.Element("IncludedShippingAreas") == null ? new List<string>() :
                    shipElement.Element("IncludedShippingAreas").Elements("Area").Select(el => el.Value),
                ExcludedShippingAreas = shipElement.Element("ExcludedShippingAreas") == null ? new List<string>() :
                    shipElement.Element("ExcludedShippingAreas").Elements("Area").Select(el => el.Value),
                FormValue = shipElement.Attribute("FormValue").Value
            } : null;
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
