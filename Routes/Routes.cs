using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Nwazet.Commerce.Routes {
    [OrchardFeature("Nwazet.Commerce")]
    public class Routes : IRouteProvider {
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
                        "stripe/checkout",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "Stripe"},
                            {"action", "Checkout"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "stripe/ship",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "Stripe"},
                            {"action", "Ship"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "stripe/summary",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "Stripe"},
                            {"action", "Summary"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "admin/usps/price",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "UspsAdmin"},
                            {"action", "Price"}
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