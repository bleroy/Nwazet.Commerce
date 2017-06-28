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

    public class ProductPriceFilter : IFilterProvider {

        public Localizer T { get; set; }
        private readonly Work<IResourceManager> _resourceManager;
        public ProductPriceFilter( Work<IResourceManager> resourceManager) {
            _resourceManager = resourceManager;

            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeFilterContext describe) {
            describe.For("ProductPart", T("Product"), T("Product"))
                .Element("ProductPrice", T("Product Price"), T("Product Price Filter."),
                    (Action<dynamic>)ApplyFilter,
                    (Func<dynamic, LocalizedString>)DisplayFilter,
                    "ProductPriceFilterForm"
                );
        }

        public void ApplyFilter(dynamic context) {
            string value = context.State.Value;
            string min = context.State.Min;
            string max = context.State.Max;
            var op = (NumericOperator)Enum.Parse(typeof(NumericOperator), Convert.ToString(context.State.Operator));
            var filterExpression = FilterHelper.GetFilterPredicateNumeric(op, "Price", value, min, max);
            var query = (IHqlQuery)context.Query;
            context.Query = query
                .Where(x => x.ContentPartRecord<ProductPartVersionRecord>(), filterExpression)
                .Where(y => y.ContentItem(), z => z.Not(w => w.InSubquery("Id", "select id from Nwazet.Commerce.Models.BundlePartRecord")));
            return;
        }

        public LocalizedString DisplayFilter(dynamic context) {
            return FilterHelper.DisplayFilterNumeric(T, context.State, "Product Price");
        }
    }
}