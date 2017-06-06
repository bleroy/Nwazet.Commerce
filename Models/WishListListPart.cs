using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nwazet.Commerce.Models {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListListPart : ContentPart<WishListListPartRecord> {
        private static readonly char[] separator = new[] { '{', '}', ',' };
        private readonly LazyField<IEnumerable<ContentItem>> _wishListElements = new LazyField<IEnumerable<ContentItem>>();
        public LazyField<IEnumerable<ContentItem>> WishListElementsField { get { return _wishListElements; } }

        private string _serializedItemIds {
            get { return Retrieve(r => r.SerializedIds); }
            set { Store(r => r.SerializedIds, value); }
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
            get { return Retrieve(r => r.IsDefault); }
            set { Store(r => r.IsDefault, value); }
        }
        
    }
}
