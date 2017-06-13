using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListListPartHandler: ContentHandler {
        private readonly IContentManager _contentManager;

        public WishListListPartHandler(IContentManager contentManager, IRepository<WishListListPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
            _contentManager = contentManager;

            OnLoading<WishListListPart>((context, part) => LazyLoadHandlers(part));
            OnVersioning<WishListListPart>((context, part, newVersionPart) => LazyLoadHandlers(newVersionPart));

            OnDestroying<WishListListPart>((context, part) => {
                repository.Delete(part.Record);
            });
        }
        
        protected void LazyLoadHandlers(WishListListPart part) {
            part.WishListElementsField.Loader(() => _contentManager.GetMany<ContentItem>(part.Ids, VersionOptions.Published, QueryHints.Empty));
        }
    }
}
