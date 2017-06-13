using Nwazet.Commerce.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.PersistentCart")]
    public class ProductsListPartDriver : ContentPartDriver<ProductsListPart>{
    }
}
