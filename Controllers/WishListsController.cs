using Nwazet.Commerce.Models;
using Nwazet.Commerce.Services;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nwazet.Commerce.Controllers {
    [OrchardFeature("Nwazet.WishLists")]
    [Themed]
    public class WishListsController : Controller {
        private readonly IWorkContextAccessor _wca;
        private readonly IWishListServices _wishListServices;
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IProductAttributeExtensionProvider> _attributeExtensionProviders;
        
        public WishListsController(
            IWorkContextAccessor wca,
            IWishListServices wishListServices,
            IContentManager contentManager,
            IEnumerable<IProductAttributeExtensionProvider> attributeExtensionProviders) {

            _wca = wca;
            _wishListServices = wishListServices;
            _contentManager = contentManager;
            _attributeExtensionProviders = attributeExtensionProviders;
        }

        private const string AttributePrefix = "productattributes.a";
        private const string ExtensionPrefix = "ext.";

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
            return View(new WishListsIndexViewModel {
                CreateShape = _wishListServices.CreateShape(user),
                SettingsShape = _wishListServices.SettingsShape(user),
                WishLists = wishLists,
                WishList = selectedList,
                WishListView = _contentManager.BuildDisplay(selectedList)
            });
        }

        [HttpPost]
        public ActionResult CreateWishList(string new_wishlist_title, int productId = 0) {
            var user = _wca.GetContext().CurrentUser;
            if (user == null) {
                return new HttpUnauthorizedResult();
            }

            var wishlistId = 0;
            //create wish list
            var wishList = _wishListServices.CreateWishList(user, new_wishlist_title);
            wishlistId = wishList.ContentItem.Id;
            //add product to wishlist
            if (productId > 0) {
                var productPart = _contentManager.Get<ProductPart>(productId);
                var form = HttpContext.Request.Form;
                var files = HttpContext.Request.Files;
                var productattributes = form.AllKeys
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

                _wishListServices.AddProductToWishList(user, wishList, productPart, productattributes);
            }
            return RedirectToAction("Index", new { id = wishlistId });
        }

        [HttpPost]
        public ActionResult AddToWishList(int productId, int wishListId = 0) {
            var user = _wca.GetContext().CurrentUser;
            if (user == null) {
                return new HttpUnauthorizedResult();
            }

            var wishlistId = 0;

            return RedirectToAction("Index", new { id = wishlistId });
        }
    }
}
