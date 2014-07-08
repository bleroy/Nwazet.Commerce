using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Nwazet.Commerce.Routes {
    [OrchardFeature("Stripe")]
    public class StripeRoutes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
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
                        "stripe/pay",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "Stripe"},
                            {"action", "Pay"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "stripe/sendmoney",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "Stripe"},
                            {"action", "SendMoney"}
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