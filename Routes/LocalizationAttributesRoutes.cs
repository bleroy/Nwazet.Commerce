using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Nwazet.Commerce.Routes {
    [OrchardFeature("Nwazet.AttributesLocalizationExtension")]
    public class LocalizationAttributesRoutes : IRouteProvider {

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var rd in GetRoutes()) {
                routes.Add(rd);
            }
        }
        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "Nwazet.Commerce/AttributesAdmin/{action}",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Commerce"},
                            {"controller", "LocalizationAttributesAdmin"},
                            {"action", "Index"}
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
