using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nwazet.Commerce.Controllers;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Filters;
using Orchard.UI.Admin;
using Orchard.UI.Resources;

namespace Nwazet.Commerce.Filters {
    [OrchardFeature("Nwazet.AttributesLocalizationExtension")]
    public class LocalizationAttributeFilter : FilterProvider, IResultFilter {

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IOrchardServices _orchardServices;

        public LocalizationAttributeFilter(IWorkContextAccessor workContextAccessor, IOrchardServices orchardServices) {
            _workContextAccessor = workContextAccessor;
            _orchardServices = orchardServices;
        }
        public void OnResultExecuted(ResultExecutedContext fiterContext) { }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            if (!(filterContext.Result is ViewResult) || 
                !(((dynamic)filterContext.Result).Model is AttributesIndexViewModel)) {
                return;
            }

            AttributesIndexViewModel model = (AttributesIndexViewModel)(((dynamic)filterContext.Result).Model);
            var workContext = _workContextAccessor.GetContext();
            if (workContext != null) {
                workContext.Layout.Body.Add(
                    _orchardServices.New.LocalizationAttributeIndexFilterScript(
                        Model: model), "0");
            }
        }

    }
}
