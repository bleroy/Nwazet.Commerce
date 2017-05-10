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
    public class MostProfitableProductsReport : ICommerceReport {
        private readonly IContentManager _contentManager;

        public MostProfitableProductsReport(IContentManager contentManager) {
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public string Name {
            get { return T("Most profitable products").Text; }
        }

        public string Description {
            get { return T("Products that made the most money.").Text; }
        }

        public string DescriptionColumnHeader {
            get { return T("Product").Text; }
        }

        public string ValueColumnHeader {
            get { return T("Revenue").Text; }
        }

        public string ValueFormat {
            get { return "c"; }
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
            var totalRevenues = new Dictionary<int, decimal>();
            foreach (var order in orders) {
                var checkoutItems = order.As<OrderPart>().Items;
                foreach (var checkoutItem in checkoutItems) {
                    var productId = checkoutItem.ProductId;
                    var totalPrice = (checkoutItem.Quantity*checkoutItem.Price) + checkoutItem.LinePriceAdjustment;
                    if (totalRevenues.ContainsKey(productId)) {
                        totalRevenues[productId] += totalPrice;
                    }
                    else {
                        totalRevenues.Add(productId, totalPrice);
                    }
                }
            }
            var products = _contentManager.GetMany<TitlePart>(
                totalRevenues.Keys,
                VersionOptions.Published,
                QueryHints.Empty)
                .ToDictionary(title => title.Id, title => title.Title);
            return new ReportData {
                DataPoints = totalRevenues
                    .Select(q => new ReportDataPoint {
                        Description = products.ContainsKey(q.Key) ? products[q.Key] : q.Key.ToString(),
                        Value = q.Value
                    })
                    .OrderByDescending(q => q.Value)
            };
        }
    }
}
