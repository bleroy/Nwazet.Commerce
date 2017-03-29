using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard;

namespace Nwazet.Commerce.Services {
    public interface ISKUGenerationServices : IDependency {

        AdvancedSKUsSiteSettingPart GetSettings();
        /// <summary>
        /// Generate a new SKU for the product. Usually, before calling this method you should check 
        /// GetSettings().GenerateSKUAutomatically. Implementations of this method are not
        /// guaranteed to not have side effects.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>The newly generated sku.</returns>
        string GenerateSku(ProductPart part);
        /// <summary>
        /// Process the SKU to handle duplicate cases. The sku of the ProductPart may change if
        /// it was not unique.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>False if the SKU of the ProductPart was found not unique and changed. True if the SKU has not changed.</returns>
        bool ProcessSku(ProductPart part);
    }
}
