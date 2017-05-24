using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListListPart : ContentPart {
        private static readonly char[] separator = new[] { '{', '}', ',' };
        private readonly LazyField<IEnumerable<ContentItem>> _wishListElements = new LazyField<IEnumerable<ContentItem>>();
        public LazyField<IEnumerable<ContentItem>> WishListElementsField { get { return _wishListElements; } }

        private string _serializedItemIds {
            get { return Retrieve<string>("SerializedItemIds"); }
            set { Store<string>("SerializedItemIds", value); }
        }

        public int[] Ids {
            get {
                return _serializedItemIds
                    .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                    .Select(frag => int.Parse(frag))
                    .ToArray();
            }
            set {
                _serializedItemIds = "{" + string.Join("},{", value) + "}";
            }
        }

        public IEnumerable<ContentItem> WishListElements {
            get {
                return _wishListElements.Value ?? Enumerable.Empty<ContentItem>();
            }
        }

        public bool IsDefault {
            get { return Retrieve<bool>("IsDefault"); }
            set { Store<bool>("IsDefault", value); }
        }

    }
}
