using System.Collections.Generic;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.ViewModels {
    public class ListOrdersViewModel {
        public ListOrdersViewModel() {
            Options = new ContentOptions();
        }

        public int? Page { get; set; }
        public IList<Entry> Entries { get; set; }
        public ContentOptions Options { get; set; }

        public class Entry {
            public ContentItem ContentItem { get; set; }
            public ContentItemMetadata ContentItemMetadata { get; set; }
        }
    }

    public class ContentOptions {
        public ContentOptions() {
            OrderBy = ContentsOrder.Modified;
            BulkAction = ContentsBulkAction.None;
        }

        public string Search { get; set; }
        public string SelectedFilter { get; set; }
        public IEnumerable<KeyValuePair<string, string>> FilterOptions { get; set; }
        public ContentsOrder OrderBy { get; set; }
        public ContentsBulkAction BulkAction { get; set; }
    }

    public enum ContentsOrder {
        Modified,
        Created
    }

    public enum ContentsBulkAction {
        None,
        Remove
    }
}