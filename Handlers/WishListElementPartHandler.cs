using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListElementPartHandler : ContentHandler{
        private readonly IContentManager _contentManager;

        public WishListElementPartHandler(IContentManager contentManager, IRepository<WishListElementPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
            _contentManager = contentManager;

            OnLoading<WishListElementPart>((context, part) => LazyLoadHandlers(part));
            OnVersioning<WishListElementPart>((context, part, newVersionPart) => LazyLoadHandlers(newVersionPart));

            OnDestroying<WishListElementPart>((context, part) => {
                repository.Delete(part.Record);
            });
        }

        protected void LazyLoadHandlers(WishListElementPart part) {
            part.WishListField.Loader(() => _contentManager.Get<WishListListPart>(part.WishListId, VersionOptions.Published, QueryHints.Empty));
        }
    }
}
