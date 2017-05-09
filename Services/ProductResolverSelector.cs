using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.Services
{
    public class ProductResolverSelector : IIdentityResolverSelector
    {
        private readonly IContentManager _contentManager;

        public ProductResolverSelector(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public IdentityResolverSelectorResult GetResolver(ContentIdentity contentIdentity)
        {
            if (contentIdentity.Has("Sku"))
            {
                return new IdentityResolverSelectorResult
                {
                    Priority = 0,
                    Resolve = ResolveIdentity
                };
            }

            return null;
        }

        private IEnumerable<ContentItem> ResolveIdentity(ContentIdentity identity)
        {
            var identifier = identity.Get("Sku");

            if (identifier == null)
            {
                return null;
            }

            return _contentManager
                .Query<ProductPart, ProductPartVersionRecord>()
                .Where(p => p.Sku == identifier)
                .List<ContentItem>()
                .Where(c => ContentIdentity.ContentIdentityEqualityComparer.AreEquivalent(
                    identity, _contentManager.GetItemMetadata(c).Identity));
        }
    }
}