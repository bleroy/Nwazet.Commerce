using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;
using System.Web;

namespace Nwazet.Commerce.Services {
    public interface IProductAttributeExtensionProvider : IDependency {
        string Name { get; }
        string DisplayName { get; }
        string Serialize(string value, Dictionary<string, string> form, HttpFileCollectionBase files);
        dynamic BuildInputShape(ProductAttributePart part);
        dynamic BuildAdminShape(string value);
        string DisplayString(string value);
    }
}
