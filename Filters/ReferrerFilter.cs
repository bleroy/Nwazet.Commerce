using System;
using System.Web.Mvc;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Filters;
using Orchard.UI.Resources;

namespace Nwazet.Commerce.Filters {
    [OrchardFeature("Nwazet.Referrals")]
    public class ReferrerFilter : FilterProvider, IActionFilter {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IResourceManager _resourceManager;

        public ReferrerFilter(IWorkContextAccessor workContextAccessor, IResourceManager resourceManager) {
            _workContextAccessor = workContextAccessor;
            _resourceManager = resourceManager;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) {
            // Referrer needs to be handled client-side so that it works even if the page is cached
            _resourceManager.Include("script", "~/Modules/Nwazet.Commerce/scripts/referral.min.js", "~/Modules/Nwazet.Commerce/scripts/referral.js");
            // Also do the work on the server-side for the current, uncached request
            var ctx = _workContextAccessor.GetContext().HttpContext;
            if (ctx != null) {
                var server = ctx.Request.ServerVariables["SERVER_NAME"];
                var referrerString = ctx.Request.ServerVariables["HTTP_REFERER"];
                if (referrerString == null) return;
                var referrer = new Uri(referrerString).Host;
                if (string.Compare(server, referrer, StringComparison.OrdinalIgnoreCase) == 0) {
                    var cookie = ctx.Request.Cookies["referrer"];
                    referrer = cookie == null ? null : cookie.Value;
                }
                ctx.Items["Nwazet.Commerce.Referrer"] = referrer;
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {
        }
    }
}