using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using System.Linq;

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
            part.WishListItemsField.Loader(() => 
                _contentManager
                .Query<WishListItemPart, WishListItemPartRecord>()
                .Where(ipr => ipr.WishListId == part.ContentItem.Id)
                .List()
                .Select(ip => ip.ContentItem)
                );
        }
    }
}
