using Nwazet.Commerce.Models;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Web;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.AttributeExtensions")]
    public class TextProductAttributeExtensionProvider : IProductAttributeExtensionProvider {

        private readonly dynamic _shapeFactory;

        public TextProductAttributeExtensionProvider(IShapeFactory shapeFactory) {
            _shapeFactory = shapeFactory;
        }

        public string Name {
            get { return "TextProductAttributeExtension"; }
        }

        public string DisplayName {
            get { return "Text Field"; }
        }

        public string Serialize(string value, Dictionary<string, string> form, HttpFileCollectionBase files) {
            return value;
        }

        public dynamic BuildInputShape(ProductAttributePart part) {
            return _shapeFactory.TextProductAttributeExtensionInput(
                ExtensionName: Name,
                Part: part);
        }

        public string DisplayString(string value) {
            return string.Format("[{0}]", value);
        }

        public dynamic BuildAdminShape(string value) {
            return _shapeFactory.TextProductAttributeExtensionAdmin();
        }
    }
}