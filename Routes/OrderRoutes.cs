using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Nwazet.Commerce.Routes {
    [OrchardFeature("Nwazet.Orders")]
    public class OrderRoutes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Route = new Route(
                        "order/summary",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "OrderSsl"},
                            {"action", "Confirmation"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "order/{id}",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "OrderSsl"},
                            {"action", "Show"}
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