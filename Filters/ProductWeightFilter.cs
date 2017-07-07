using System;
using Nwazet.Commerce.Extensions;
using Nwazet.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.Environment;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.FilterEditors.Forms;
using Orchard.Projections.Services;
using Orchard.UI.Resources;

namespace Nwazet.Commerce.Filters {

    public class ProductWeightFilter : IFilterProvider {
        public Localizer T { get; set; }
        private readonly Work<IResourceManager> _resourceManager;
        public ProductWeightFilter( Work<IResourceManager> resourceManager) {
            _resourceManager = resourceManager;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeFilterContext describe) {
            describe.For("ProductPart", T("Product"), T("Product"))
                .Element("ProductWeight", T("Product Weight"), T("Product Weight Filter."),
                    (Action<dynamic>)ApplyFilter,
                    (Func<dynamic, LocalizedString>)DisplayFilter,
                    "ProductWeightFilterForm"
                );
        }

        public void ApplyFilter(dynamic context) {
            string value = context.State.Value;
            string min = context.State.Min;
            string max = context.State.Max;
            var op = (NumericOperator)Enum.Parse(typeof(NumericOperator), Convert.ToString(context.State.Operator));
            var filterExpression = FilterHelper.GetFilterPredicateNumeric(op, "Weight", value, min, max);
            var query = (IHqlQuery)context.Query;
            context.Query = query
                    .Where(x => x.ContentPartRecord<ProductPartVersionRecord>(), filterExpression);
            return;
        }

        public LocalizedString DisplayFilter(dynamic context) {
            return FilterHelper.DisplayFilterNumeric(T, context.State, "Product Weight"); 
        }
    }
}