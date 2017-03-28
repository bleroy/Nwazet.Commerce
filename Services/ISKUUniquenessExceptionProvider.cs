using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    public interface ISKUUniquenessExceptionProvider : IDependency {
        /// <summary>
        /// This method returns the Ids of he ProductParts that are allowed to have the same SKU as the part
        /// passed as parameter.
        /// </summary>
        /// <param name="part">The ProductPart of which we want to find the valid SKU duplicates.</param>
        /// <returns>The Ids of ProductParts that are allowed to have the same SKU.</returns>
        IEnumerable<int> GetIdsOfValidSKUDuplicates(ProductPart part);
    }
}
