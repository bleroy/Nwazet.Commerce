﻿using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using System.Collections.Generic;

namespace Nwazet.Commerce.ViewModels {
    public class ProductAttributePartEditViewModel {
        public ProductAttributePart Part { get; set; }
        public IEnumerable<IProductAttributeExtensionProvider> AttributeExtensionProviders { get; set; }
    }
}
