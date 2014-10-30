﻿using System.Collections.Generic;
using Nwazet.Commerce.Models;

namespace Nwazet.Commerce.ViewModels {
    public class ProductAttributePartDisplayViewModel {
        public ProductAttributePart Part { get; set; }
        public IEnumerable<dynamic> AttributeExtensionShapes { get; set; }
    }
}
