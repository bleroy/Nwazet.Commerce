using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Models.Reporting;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Localization;

namespace Nwazet.Commerce.Reports {
    public abstract class BaseSalesReport : ICommerceReport {
        private readonly IContentManager _contentManager;

        protected BaseSalesReport(IContentManager contentManager) {
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract string DescriptionColumnHeader { get; }
        public abstract string ValueColumnHeader { get; }
        public abstract string ValueFormat { get; }

        public abstract ChartType ChartType { get; }

        public abstract double ComputeResultForInterval(IList<OrderPart> ordersForInterval);

        public IEnumerable<ReportDataPoint> GetData(DateTime startDate, DateTime endDate, TimePeriod granularity) {
            startDate = granularity.BeginningDate(startDate);
            endDate = granularity.EndingDate(endDate);
            var orders = _contentManager
                .Query<CommonPart, CommonPartRecord>("Order")
                .Where(r =>
                    r.CreatedUtc >= startDate.ToUniversalTime()
                    && r.CreatedUtc <= endDate.ToUniversalTime())
                .OrderBy(r => r.CreatedUtc)
                .Join<OrderPartRecord>()
                .Where(order => order.Status != OrderPart.Cancelled)
                .List()
                .ToList();
            var numberOfPoints = granularity.PeriodCount(startDate, endDate);
            var results = new List<ReportDataPoint>(numberOfPoints);
            var intervalStart = startDate;
            var intervalEnd = startDate + granularity;
            while (intervalStart < endDate) {
                var ordersForInterval = orders.Where(
                    common => common.CreatedUtc >= intervalStart
                              && common.CreatedUtc < intervalEnd)
                    .Select(common => common.As<OrderPart>())
                    .ToList();
                results.Add(new ReportDataPoint {
                    Description = granularity.ToString(intervalStart, CultureInfo.CurrentUICulture),
                    Value = ComputeResultForInterval(ordersForInterval)
                });
                intervalStart = intervalEnd;
                intervalEnd = intervalStart + granularity;
            }
            return results;
        }
    }
}
