using System;
using System.Collections.Generic;
using Orchard;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Services {
    [OrchardFeature("Nwazet.Referrals")]
    public class ReferrerCartInfoProvider : IExtraCartInfoProvider {
        private readonly IWorkContextAccessor _workContextAccessor;

        public ReferrerCartInfoProvider(IWorkContextAccessor workContextAccessor) {
            _workContextAccessor = workContextAccessor;
        }

        public IEnumerable<string> GetExtraCartInfo() {
            var ctx = _workContextAccessor.GetContext().HttpContext;
            if (ctx != null) {
                var referrer = ctx.Items["Nwazet.Commerce.Referrer"] as string;
                if (!string.IsNullOrWhiteSpace(referrer) &&
                    string.Compare("null", referrer, StringComparison.OrdinalIgnoreCase) != 0) {

                    return new[] {"Referrer: " + referrer};
                }
            }
            return new string[] {};
        }
    }
}
