using System;
using System.Collections.Generic;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Models.Reporting;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Localization;

namespace Nwazet.Commerce.Reports {
    public class BestSellingProductsReport : ICommerceReport {
        private readonly IContentManager _contentManager;

        public BestSellingProductsReport(IContentManager contentManager) {
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public string Name {
            get { return T("Best Sellers").Text; }
        }

        public string Description {
            get { return T("Best selling products as a percentage of the total number of products sold.").Text; }
        }

        public string DescriptionColumnHeader {
            get { return T("Product").Text; }
        }

        public string ValueColumnHeader {
            get { return T("Percentage of total number of products sold").Text; }
        }

        public string ValueFormat {
            get { return "p"; }
        }

        public ChartType ChartType {
            get { return ChartType.Doughnut; }
        }

        public ReportData GetData(DateTime startDate, DateTime endDate, TimePeriod granularity) {
            var orders = _contentManager
                .Query<CommonPart, CommonPartRecord>("Order")
                .Where(r =>
                    r.CreatedUtc >= startDate.ToUniversalTime()
                    && r.CreatedUtc <= endDate.ToUniversalTime())
                .Join<OrderPartRecord>()
                .Where(order => order.Status != OrderPart.Cancelled)
                .List()
                .ToList();
            var totalQuantities = new Dictionary<int, int>();
            foreach (var order in orders) {
                var checkoutItems = order.As<OrderPart>().Items;
                foreach (var checkoutItem in checkoutItems) {
                    var productId = checkoutItem.ProductId;
                    if (totalQuantities.ContainsKey(productId)) {
                        totalQuantities[productId] += checkoutItem.Quantity;
                    }
                    else {
                        totalQuantities.Add(productId, checkoutItem.Quantity);
                    }
                }
            }
            var totalProductsSold = totalQuantities.Values.Sum();
            var products = _contentManager.GetMany<TitlePart>(
                totalQuantities.Keys,
                VersionOptions.Published,
                QueryHints.Empty)
                .ToDictionary(title => title.Id, title => title.Title);
            return new ReportData {
                DataPoints = totalQuantities
                    .Select(q => new ReportDataPoint {
                        Description = products[q.Key],
                        Value = q.Value,
                        ValueString = T("{0:p} ({1})", (double) q.Value/totalProductsSold, q.Value).Text
                    })
                    .OrderByDescending(q => q.Value)
            };
        }
    }
}
