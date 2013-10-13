using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Nwazet.Commerce.Routes {
    [OrchardFeature("Usps.Shipping")]
    public class UspsRoutes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
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