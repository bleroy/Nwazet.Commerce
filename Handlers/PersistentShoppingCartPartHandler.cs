using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace Nwazet.Commerce.Handlers {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class PersistentShoppingCartPartHandler : ContentHandler {
        private readonly IContentManager _contentManager;

        public PersistentShoppingCartPartHandler(
            IContentManager contentManager
            ) {

            _contentManager = contentManager;
        }
        
    }
}
