﻿using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using System.Collections.Generic;

namespace Nwazet.Commerce.ViewModels {
    public class ProductAttributePartEditViewModel {
        public IEnumerable<ProductAttributeValue> AttributeValues { get; set; }
        public int SortOrder { get; set; }
        public string DisplayName { get; set; }
        public string TechnicalName { get; set; }
        public IEnumerable<IProductAttributeExtensionProvider> AttributeExtensionProviders { get; set; }
    }
}
