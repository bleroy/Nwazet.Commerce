using Nwazet.Commerce.Models;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Workflows.Services;
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
        private readonly IWishListsUIServices _wishListsUIServices;
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IProductAttributeExtensionProvider> _attributeExtensionProviders;
        private readonly IShoppingCart _shoppingCart;
        private readonly IWorkflowManager _workflowManager;
        private readonly IEnumerable<IWishListExtensionProvider> _wishListExtensionProviders;
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;

        public WishListsController(
            IWorkContextAccessor wca,
            IWishListServices wishListServices,
            IWishListsUIServices wishListsUIServices,
            IContentManager contentManager,
            IEnumerable<IProductAttributeExtensionProvider> attributeExtensionProviders,
            IShoppingCart shoppingCart,
            IWorkflowManager workflowManager,
            IEnumerable<IWishListExtensionProvider> wishListExtensionProviders,
            IOrchardServices orchardServices,
            IMembershipService membershipService) {

            _wca = wca;
            _wishListServices = wishListServices;
            _wishListsUIServices = wishListsUIServices;
            _contentManager = contentManager;
            _attributeExtensionProviders = attributeExtensionProviders;
            _shoppingCart = shoppingCart;
            _workflowManager = workflowManager;
            _wishListExtensionProviders = wishListExtensionProviders;
            _orchardServices = orchardServices;
            _membershipService = membershipService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private const string AttributePrefix = "productattributes.a";
        private const string ExtensionPrefix = "ext.";

        [OutputCache(Duration = 0)]
        public ActionResult Index(string uName, int id = 0) {
            //the uName string is there to allow accessing other user's wishlists
            var user = string.IsNullOrWhiteSpace(uName) ? _wca.GetContext().CurrentUser
                : _membershipService.GetUser(uName);

            WishListListPart selectedList;
            if (_wishListServices.TryGetWishList(out selectedList, id)) {
                if (!_orchardServices.Authorizer.Authorize(WishListPermissions.ViewWishLists, selectedList))
                    return RedirectToAction("Index", new { id = 0 }); //redirect to own wish lists
                return View(_contentManager.BuildDisplay(selectedList));
            }

            return View(_contentManager.BuildDisplay(_wishListServices.GetDefaultWishList(_wca.GetContext().CurrentUser)));
        }

        public ActionResult Create() {
            var model = _wishListsUIServices.CreateShape(_wca.GetContext().CurrentUser);
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
            var model = _wishListsUIServices.SettingsShape(_wca.GetContext().CurrentUser, wishListId);
            return View("WishListsSettings", model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult UpdateSettings(
            int wishListId, int defaultId,
            IDictionary<int, string> newTitles, IEnumerable<int> wishListsToDelete) {
            var user = _wca.GetContext().CurrentUser;

            var wishLists = _wishListServices.GetWishLists(user);
            foreach (var wishList in wishLists) {
                //these are the wishlists for the current user, so we don't need to evaluate their permissions
                var wlId = wishList.ContentItem.Id;
                if (wishListsToDelete?.Contains(wlId) == true) {
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
            WishListListPart wishList;
            if (_wishListServices.TryGetWishList(out wishList, id)) {
                if (!_orchardServices.Authorizer.Authorize(WishListPermissions.DeleteWishLists, wishList))
                    return RedirectToAction("Index", new { id = 0 }); //redirect to own wish lists

                _wishListServices.DeleteWishlist(wishList);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddToWishList(int wishListId, int productId) {
            var user = _wca.GetContext().CurrentUser;
            WishListListPart wishList;
            if (_wishListServices.TryGetWishList(out wishList, wishListId)) {
                if (!_orchardServices.Authorizer.Authorize(WishListPermissions.UpdateWishLists, wishList))
                    return RedirectToAction("Index", new { id = 0 }); //redirect to own wish lists
            } else {//the case we are trying to add to the default wishlist also falls here
                wishList = _wishListServices.GetWishList(user, wishListId);
            }
            AddProduct(user, wishList, productId);

            return RedirectToAction("Index", new { id = wishList.ContentItem.Id });
        }

        [HttpPost]
        public ActionResult AddToCart(int wishListItemId, int wishListId, int quantity = 1) {

            WishListListPart wishList;
            if (_wishListServices.TryGetWishList(out wishList, wishListId)) {
                if (wishList.Ids.Contains(wishListItemId)) {
                    //these checks reduce the likelihood that we get here even though someone has tampered with the page.
                    var wishListItem = _contentManager.Get<WishListItemPart>(wishListItemId);
                    if (wishListItem != null) {
                        _shoppingCart.Add(wishListItem.Item.ProductId, quantity, wishListItem.Item.AttributeIdsToValues);

                        _workflowManager.TriggerEvent("CartUpdated",
                            _wca.GetContext().CurrentSite,
                            () => new Dictionary<string, object> {
                                {"Cart", _shoppingCart}
                            });


                        return RedirectToAction("Index", new { controller = "ShoppingCart" });
                    }
                }
            }
            //in case we failed adding the product to the cart
            return RedirectToAction("Index", new { id = wishList != null ? wishList.ContentItem.Id : 0 });
        }

        [HttpPost]
        public ActionResult RemoveFromWishList(int wishListId, int itemId) {
            WishListListPart wishList;
            if (_wishListServices.TryGetWishList(out wishList, wishListId)) {
                if (!_orchardServices.Authorizer.Authorize(WishListPermissions.UpdateWishLists, wishList))
                    return RedirectToAction("Index", new { id = 0 }); //redirect to own wish lists

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
