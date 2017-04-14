using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface IProductAttributeNameService : IDependency {
        string GenerateAttributeTechnicalName(string displayName);

        bool ProcessTechnicalName(ProductAttributePart part);
    }
}
