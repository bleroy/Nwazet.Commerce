using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Models.Reporting;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Nwazet.Commerce.Reports {
    [OrchardFeature("Nwazet.Reports")]
    public class AverageSaleAmountReport : BaseSalesReport {
        public AverageSaleAmountReport(IContentManager contentManager) : base(contentManager) {
        }

        public override string Name {
            get { return T("Average order").Text; }
        }

        public override string Description {
            get { return T("Average amount of one order over the period").Text; }
        }

        public override string DescriptionColumnHeader {
            get { return T("Period").Text; }
        }

        public override string ValueColumnHeader {
            get { return T("Average amount of orders").Text; }
        }

        public override string ValueFormat {
            get { return "c"; }
        }

        public override ChartType ChartType {
            get { return ChartType.Line; }
        }

        public override decimal ComputeResultForInterval(IList<OrderPart> ordersForInterval) {
            return ordersForInterval.Any()
                ? ordersForInterval.Average(order => order.As<OrderPart>().AmountPaid)
                : 0.0M;
        }
    }
}
