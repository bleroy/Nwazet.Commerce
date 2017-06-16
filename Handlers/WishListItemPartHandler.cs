using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListItemPartHandler : ContentHandler{
        private readonly IContentManager _contentManager;

        public WishListItemPartHandler(IContentManager contentManager, IRepository<WishListItemPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
            _contentManager = contentManager;

            OnLoading<WishListItemPart>((context, part) => LazyLoadHandlers(part));
            OnVersioning<WishListItemPart>((context, part, newVersionPart) => LazyLoadHandlers(newVersionPart));

            OnDestroying<WishListItemPart>((context, part) => {
                repository.Delete(part.Record);
            });
        }

        protected void LazyLoadHandlers(WishListItemPart part) {
            part.WishListField.Loader(() => _contentManager.Get<WishListListPart>(part.WishListId, VersionOptions.Published, QueryHints.Empty));
        }
    }
}
