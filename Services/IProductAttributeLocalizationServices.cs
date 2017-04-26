using System;
using System.Collections.Generic;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.Localization.Models;

namespace Nwazet.Commerce.Services {
    public interface IProductAttributeLocalizationServices : IDependency {
        /// <summary>
        /// Returns an enumerable of pairs of Ids. Each tuple
        /// from the enumerable has the form:
        /// Item1 is the attribute id in the initial ProductAttributesPart
        /// Item2 is the attribute id after localization
        /// Items 2 may be negative in the case where no localization to the target culture was found.
        /// </summary>
        /// <param name="locPart">A LocalizationPart for the desired target culture.</param>
        /// <returns>A IEnumerable as described.</returns>
        IEnumerable<Tuple<int, int>> GetLocalizationIdPairs(ProductAttributesPart attributesPart, LocalizationPart locPart);
        /// <summary>
        /// Returns an IEnumerable containing the ProductAttributeParts that are selected in the given ProductAttributesPart
        /// and whose culture does not match the culture from the give LocalizationPart
        /// </summary>
        /// <param name="attributesPart">THe ProductAttributesPart whose selected attributes will be verified</param>
        /// <param name="locPart">The LocalizationPart with the base culture we are looking for.</param>
        /// <returns>A IEnumerable as described</returns>
        IEnumerable<ProductAttributePart> GetAttributesInTheWrongCulture(ProductAttributesPart attributesPart, LocalizationPart locPart);
    }
}
