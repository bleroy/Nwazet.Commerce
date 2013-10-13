using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Nwazet.Commerce.Routes {
    [OrchardFeature("Nwazet.Commerce")]
    public class CommerceRoutes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Route = new Route(
                        "cart",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "ShoppingCart"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "nakedcart",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "ShoppingCart"},
                            {"action", "NakedCart"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "getcart",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "ShoppingCart"},
                            {"action", "GetItems"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "addtocart",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "ShoppingCart"},
                            {"action", "Add"},
                            {"productattributes", null}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}