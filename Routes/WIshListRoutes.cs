using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nwazet.Commerce.Routes {
    [OrchardFeature("Nwazet.WishLists")]
    public class WishListRoutes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Route = new Route(
                        "wishlists/new/{productId}",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "WishLists"},
                            {"action", "Create"},
                            {"productId", 0 }
                        },
                        new RouteValueDictionary {
                            {"productId", @"\d*" }
                        },
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "wishlists/add/{wishListId}/{productId}",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "WishLists"},
                            {"action", "AddToWishList"},
                            {"productId", 0 },
                            {"wishListId", 0 }
                        },
                        new RouteValueDictionary {
                            {"productId", @"\d*" },
                            {"wishListId", @"\d*" }
                        },
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "wishlists/remove/{wishListId}/{itemId}",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "WishLists"},
                            {"action", "RemoveFromWishList"},
                            {"itemId", 0 },
                            {"wishListId", 0 }
                        },
                        new RouteValueDictionary {
                            {"itemId", @"\d*" },
                            {"wishListId", @"\d*" }
                        },
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "wishlists/settings/{wishListId}",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "WishLists"},
                            {"action", "UpdateSettings"},
                            {"wishListId", 0 }
                        },
                        new RouteValueDictionary {
                            {"wishListId", @"\d*" }
                        },
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "wishlists/{id}",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "WishLists"},
                            {"action", "Index"},
                            {"id", 0 }
                        },
                        new RouteValueDictionary {
                            {"id", @"\d*" }
                        },
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                }
            };
        }

    }
}
