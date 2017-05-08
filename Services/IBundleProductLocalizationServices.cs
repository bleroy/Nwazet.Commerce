using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwazet.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization.Models;

namespace Nwazet.Commerce.Services {
    public interface IBundleProductLocalizationServices : IDependency {
        /// <summary>
        /// Tells whether the culture in the ContentItem is different from the culture in the 
        /// LocalizationPart
        /// </summary>
        /// <param name="ci">The ContentItem to be analyzed.</param>
        /// <param name="locPart">The LocalizationPart to be used as reference.</param>
        /// <returns>True if the ContentItem does not have the same culture as the LocalizationPart</returns>
        bool HasDifferentCulture(IContent ci, LocalizationPart locPart);
        /// <summary>
        /// Tells whether the LocalizationPart is valid and can be used for comparisons related to 
        /// localization.
        /// </summary>
        /// <param name="part">The LocalizationPart to be analyzed</param>
        /// <returns>True if the LocalizationPart can safely be used in comparisons.</returns>
        bool ValidLocalizationPart(LocalizationPart part);

        /// <summary>
        /// Returns an IEnumerable containing the content items that are selected in the given BundlePart
        /// and whose culture does not match the culture from the given LocalizationPart
        /// </summary>
        /// <param name="bundlePart">The BundlePart whose selected products will be verified</param>
        /// <param name="locPart">The LocalizationPart with the base culture we are looking for.</param>
        /// <returns>A IEnumerable as described</returns>
        IEnumerable<IContent> GetProductsInTheWrongCulture(BundlePart bundlePart, LocalizationPart locPart);
        /// <summary>
        /// Returns an IEnumerable containing the products from the BundlePart
        /// whose culture does not match the culture from the given LocalizationPart
        /// </summary>
        /// <param name="productIds">The ids of products of the BundlePart that will be verified</param>
        /// <param name="locPart">The LocalizationPart with the base culture we are looking for.</param>
        /// <returns>A IEnumerable as described</returns>
        IEnumerable<IContent> GetProductsInTheWrongCulture(IEnumerable<int> productIds, LocalizationPart locPart);

        /// <summary>
        /// Returns an enumerable of pairs of <ProductQantity, int>. Each struct
        /// from the enumerable has the form:
        /// OriginalProduct is the productQUantity id in the initial BundlePart
        /// NewProductID is the product id after localization
        /// NewProductId may be negative in the case where no localization to the target culture was found.
        /// </summary>
        /// <param name="bundlePart">The BundlePart whose selected products will be verified</param>
        /// <param name="locPart">A LocalizationPart for the desired target culture.</param>
        /// <returns>A IEnumerable as described.</returns>
        IEnumerable<ProductQuantityPair> GetLocalizationIdPairs(BundlePart bundlePart, LocalizationPart locPart);
        /// <summary>
        /// Returns an enumerable of pairs of <ProductQantity, int>. Each struct
        /// from the enumerable has the form:
        /// OriginalProduct is the productQUantity id in the initial BundlePart
        /// NewProductID is the product id after localization
        /// NewProductId may be negative in the case where no localization to the target culture was found.
        /// </summary>
        /// <param name="originalProducts">The products of the BundlePart that will be verified</param>
        /// <param name="locPart">A LocalizationPart for the desired target culture.</param>
        /// <returns>A IEnumerable as described.</returns>
        IEnumerable<ProductQuantityPair> GetLocalizationIdPairs(IEnumerable<ProductQuantity> originalProducts, LocalizationPart locPart);

    }

    public struct ProductQuantityPair {
        public ProductQuantity OriginalProduct;
        public int NewProductId;

        public ProductQuantityPair(ProductQuantity op, int np) {
            OriginalProduct = op;
            NewProductId = np;
        }
    }
}
