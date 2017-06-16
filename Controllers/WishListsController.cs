using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Workflows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Nwazet.WishLists")]
    [Themed]
    [Authorize]
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

            var selectedList = _wishListServices.GetWishList(user, id);
            return View(_contentManager.BuildDisplay(selectedList));
        }

        public ActionResult Create() {
            var model = _wishListServices.CreateShape(_wca.GetContext().CurrentUser);
            return View("WishListEditor", model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePost(string title, int productId = 0) {
            return (CreateWishList(title, productId));
        }

        [HttpPost]
        public ActionResult CreateWishList(string title, int productId = 0) {
            var user = _wca.GetContext().CurrentUser;

            var wishlistId = 0;
            //create wish list
            var wishList = _wishListServices.CreateWishList(user, title);
            wishlistId = wishList.ContentItem.Id;
            //add product to wishlist
            AddProduct(user, wishList, productId);

            return RedirectToAction("Index", new { id = wishlistId });
        }

        public ActionResult Edit(int wishListId = 0) {
            var model = _wishListServices.SettingsShape(_wca.GetContext().CurrentUser, wishListId);
            return View("WishListsSettings", model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(int wishListId, int defaultId) {
            return UpdateSettings(wishListId, defaultId);
        }

        [HttpPost]
        public ActionResult UpdateSettings(int wishListId, int defaultId) {
            var user = _wca.GetContext().CurrentUser;

            var wishLists = _wishListServices.GetWishLists(user);
            //1. Read what should change from the form
            var form = HttpContext.Request.Form;
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
                    _wishListServices.DeleteWishlist(wishList);
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

            return RedirectToAction("Index", new { id = wishListId });
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddToWishList(int wishListId, int productId) {
            var user = _wca.GetContext().CurrentUser;
            //get selected wishlist
            var wishList = _wishListServices.GetWishList(user, wishListId);

            AddProduct(user, wishList, productId);

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
        public ActionResult RemoveFromWishList(int wishListId, int itemId) {
            var user = _wca.GetContext().CurrentUser;
            //get selected wishlist
            WishListListPart wishList;
            if (_wishListServices.TryGetWishList(user, out wishList, wishListId)) {
                _wishListServices.RemoveItemFromWishlist(wishList, itemId);
            }

            return RedirectToAction("Index", new { id = wishListId });
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

        private void AddProduct(IUser user, WishListListPart wishList, int productId) {
            if (productId > 0) {
                var productPart = _contentManager.Get<ProductPart>(productId);
                if (productPart != null) {
                    var productattributes = ParseProductAttributes();
                    _wishListServices.AddProductToWishList(user, wishList, productPart, productattributes);
                }
            }
        }
    }
}
