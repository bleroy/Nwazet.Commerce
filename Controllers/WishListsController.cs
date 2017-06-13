using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using Orchard.Themes;
using Orchard.Workflows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Nwazet.WishLists")]
    [Themed]
    public class WishListsController : Controller {
        private readonly IWorkContextAccessor _wca;
        private readonly IWishListServices _wishListServices;
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IProductAttributeExtensionProvider> _attributeExtensionProviders;
        private readonly IShoppingCart _shoppingCart;
        private readonly IWorkflowManager _workflowManager;
        private readonly IEnumerable<IWishListExtensionProvider> _wishListExtensionProviders;

        public WishListsController(
            IWorkContextAccessor wca,
            IWishListServices wishListServices,
            IContentManager contentManager,
            IEnumerable<IProductAttributeExtensionProvider> attributeExtensionProviders,
            IShoppingCart shoppingCart,
            IWorkflowManager workflowManager,
            IEnumerable<IWishListExtensionProvider> wishListExtensionProviders) {

            _wca = wca;
            _wishListServices = wishListServices;
            _contentManager = contentManager;
            _attributeExtensionProviders = attributeExtensionProviders;
            _shoppingCart = shoppingCart;
            _workflowManager = workflowManager;
            _wishListExtensionProviders = wishListExtensionProviders;
        }

        private const string AttributePrefix = "productattributes.a";
        private const string ExtensionPrefix = "ext.";

        [OutputCache(Duration = 0)]
        public ActionResult Index(int id = 0) {
            var user = _wca.GetContext().CurrentUser;
            if (user == null) {
                return new HttpUnauthorizedResult();
            }
            var wishLists = _wishListServices.GetWishLists(user);
            var selectedList = wishLists.SingleOrDefault(wp => wp.ContentItem.Id == id);
            if (selectedList == null) {
                selectedList = wishLists.SingleOrDefault(wp => wp.IsDefault);
            }
            return View(_contentManager.BuildDisplay(selectedList));
        }

        [HttpPost]
        public ActionResult CreateWishList(string new_wishlist_title, int productid = 0) {
            var user = _wca.GetContext().CurrentUser;
            if (user == null) {
                return new HttpUnauthorizedResult();
            }

            var wishlistId = 0;
            //create wish list
            var wishList = _wishListServices.CreateWishList(user, new_wishlist_title);
            wishlistId = wishList.ContentItem.Id;
            //add product to wishlist
            if (productid > 0) {
                var productPart = _contentManager.Get<ProductPart>(productid);
                var productattributes = ParseProductAttributes();

                _wishListServices.AddProductToWishList(user, wishList, productPart, productattributes);
            }

            return RedirectToAction("Index", new { id = wishlistId });
        }

        [HttpPost]
        public ActionResult AddToWishList(int wishListid, int productid) {
            var user = _wca.GetContext().CurrentUser;
            if (user == null) {
                return new HttpUnauthorizedResult();
            }
            //get selected wishlist
            var wishList = _wishListServices.GetWishList(user, wishListid);

            if (productid > 0) {
                var productPart = _contentManager.Get<ProductPart>(productid);
                var productattributes = ParseProductAttributes();

                _wishListServices.AddProductToWishList(user, wishList, productPart, productattributes);
            }

            return RedirectToAction("Index", new { id = wishList.ContentItem.Id });
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity = 1) {

            //read attributes from the form
            var form = HttpContext.Request.Form;
            var productattributes = form.AllKeys
                .Where(key => key.StartsWith("attributeKey"))
                .Select(key => int.Parse(form[key]))
                .ToDictionary(
                    key => key, //id of the attribute
                    key => { //ProductAttributeValueExtended
                        return new ProductAttributeValueExtended {
                            Value = form["value_" + key],
                            ExtendedValue = form["ExtendedValue_" + key],
                            ExtensionProvider = form["ExtensionProvider_" + key]
                        };
                    }
                );

            _shoppingCart.Add(productId, quantity, productattributes);

            _workflowManager.TriggerEvent("CartUpdated",
                _wca.GetContext().CurrentSite,
                () => new Dictionary<string, object> {
                    {"Cart", _shoppingCart}
                });

            return RedirectToAction("Index", new { controller = "ShoppingCart" });
        }

        [HttpPost]
        public ActionResult RemoveFromWishList(int wishListid, int elementId) {
            var user = _wca.GetContext().CurrentUser;
            if (user == null) {
                return new HttpUnauthorizedResult();
            }
            //get selected wishlist
            var wishList = _wishListServices.GetWishList(user, wishListid);

            if (wishList.ContentItem.Id == wishListid) { //GetWIshList may return the default wish list
                _wishListServices.RemoveElementFromWishlist(user, wishList, elementId);
            }

            return RedirectToAction("Index", new { id = wishListid });
        }

        [HttpPost]
        public ActionResult UpdateSettings(int wishListid) {
            var user = _wca.GetContext().CurrentUser;
            if (user == null) {
                return new HttpUnauthorizedResult();
            }
            var wishLists = _wishListServices.GetWishLists(user);
            //1. Read what should change from the form
            var form = HttpContext.Request.Form;
            //1.1 Get the id of the new default wish list
            var defaultId = int.Parse(form["is-default-wishlist"]);
            //1.2 Get the new titles
            var newTitles = form.AllKeys
                .Where(key => key.StartsWith("wishlist-title-"))
                .ToDictionary(
                    key => int.Parse(key.Substring("wishlist-title-".Length)),
                    key => form[key]
                );
            //1.3 Get the list of wish lists to delete
            var toDelete = new List<int>();
            if (form.AllKeys.Contains("delete-wishlist")) {
                toDelete.AddRange(form["delete-wishlist"]
                    .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse));
            }
            foreach (var wishList in wishLists) {
                var wlId = wishList.ContentItem.Id;
                if (toDelete.Contains(wlId)) {
                    //Delete this list
                    _wishListServices.DeleteWishlist(user, wishList);
                } else {
                    //2. Update titles
                    var title = newTitles[wlId];
                    wishList.ContentItem.As<TitlePart>().Title = title;
                    //3. Update default wishlist
                    wishList.IsDefault = wlId == defaultId;
                    //4. Process extension behaviours
                    foreach (var ext in _wishListExtensionProviders) {
                        ext.UpdateSettings(user, wishList);
                    }
                }
            }
            //we may have deleted the default wishlist
            //that condition is handled in the GetDefaultWishList method

            return RedirectToAction("Index", new { id = wishListid });
        }

        private Dictionary<int, ProductAttributeValueExtended> ParseProductAttributes() {
            var form = HttpContext.Request.Form;
            var files = HttpContext.Request.Files;
            return form.AllKeys
               .Where(key => key.StartsWith(AttributePrefix))
               .ToDictionary(
                   key => int.Parse(key.Substring(AttributePrefix.Length)),
                   key => {
                       var extensionProvider = _attributeExtensionProviders.SingleOrDefault(e => e.Name == form[ExtensionPrefix + key + ".provider"]);
                       Dictionary<string, string> extensionFormValues = null;
                       if (extensionProvider != null) {
                           extensionFormValues = form.AllKeys.Where(k => k.StartsWith(ExtensionPrefix + key + "."))
                               .ToDictionary(
                                   k => k.Substring((ExtensionPrefix + key + ".").Length),
                                   k => form[k]);
                           return new ProductAttributeValueExtended {
                               Value = form[key],
                               ExtendedValue = extensionProvider.Serialize(form[ExtensionPrefix + key], extensionFormValues, files),
                               ExtensionProvider = extensionProvider.Name
                           };
                       }
                       return new ProductAttributeValueExtended {
                           Value = form[key],
                           ExtendedValue = null,
                           ExtensionProvider = null
                       };
                   });
        }
    }
}
