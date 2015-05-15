using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.Models.Reporting;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Common.ViewModels;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Localization.Services;
using Orchard.UI.Admin;

namespace Nwazet.Commerce.Controllers {
    [Admin]
    [OrchardFeature("Nwazet.Reports")]
    public class ReportController : Controller {
        private readonly IEnumerable<ICommerceReport> _reports;
        private readonly IOrchardServices _orchardServices;
        private readonly IDateLocalizationServices _dateServices;
        private readonly IContentManager _contentManager;

        public ReportController(
            IEnumerable<ICommerceReport> reports,
            IOrchardServices orchardServices,
            IDateLocalizationServices dateServices,
            IContentManager contentManager
            ) {
            _reports = reports;
            _orchardServices = orchardServices;
            _dateServices = dateServices;
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index() {
            if (!_orchardServices.Authorizer.Authorize(ReportPermissions.GenerateReports, null, T("Cannot generate reports"))) {
                return new HttpUnauthorizedResult();
            }
            var reports = _reports.ToList();
            return View(reports);
        }

        public ActionResult Report(string report, DateTime? startDate, DateTime? endDate, string granularity, string preset) {
            var now = DateTime.Now;
            switch (preset) {
                case "today":
                    return Report(report, TimePeriod.Day.BeginningDate(now), now, TimePeriod.Hour);
                case "thisweek":
                    return Report(report, TimePeriod.Week.BeginningDate(now), now, TimePeriod.Day);
                case "currentmonth":
                    return Report(report, TimePeriod.Month.BeginningDate(now), now, TimePeriod.Day);
                case "lastfiveyears":
                    return Report(report, TimePeriod.Year.BeginningDate(now).AddYears(-4), now, TimePeriod.Year);
                case "yeartodate":
                    return Report(report, TimePeriod.Year.BeginningDate(now), now, TimePeriod.Month);
            }
            if (startDate == null || endDate == null) {
                return Report(report, TimePeriod.Year.BeginningDate(now), now, TimePeriod.Month);
            }
            var parsedGranularity = TimePeriod.Parse(granularity);
            return Report(report, startDate.Value, endDate.Value, parsedGranularity);
        }

        private ActionResult Report(string report, DateTime startDate, DateTime endDate, TimePeriod granularity) {
            if (!_orchardServices.Authorizer.Authorize(ReportPermissions.GenerateReports, null, T("Cannot generate reports"))) {
                return new HttpUnauthorizedResult();
            }
            var reportService = _reports.FirstOrDefault(r => r.Name == report);
            if (reportService == null) {
                return HttpNotFound(T("Report {0} not found", report).Text);
            }
            var data = reportService.GetData(startDate, endDate, granularity);
            var series = data.Series;
            var model = new ReportDataViewModel {
                Name = reportService.Name,
                Description = reportService.Description,
                DescriptionColumnHeader = reportService.DescriptionColumnHeader,
                ValueColumnHeader = reportService.ValueColumnHeader,
                ValueFormat = reportService.ValueFormat,
                ChartType = reportService.ChartType,
                DataPoints = data.DataPoints,
                Series = series,
                StartDateEditor = new DateTimeEditor {
                    Date = _dateServices.ConvertToLocalizedDateString(startDate.ToUniversalTime()),
                    ShowDate = true,
                    ShowTime = false
                },
                EndDateEditor = new DateTimeEditor {
                    Date = _dateServices.ConvertToLocalizedDateString(endDate.ToUniversalTime()),
                    ShowDate = true,
                    ShowTime = false
                },
                Granularity = granularity
            };
            return View("Detail", model);
        }

        public ActionResult Export(DateTime? startDate, DateTime? endDate) {
            var orders = _contentManager
                .Query<OrderPart, OrderPartRecord>("Order")
                .Where(order => order.Status != OrderPart.Cancelled)
                .Join<CommonPartRecord>()
                .Where(r =>
                    r.CreatedUtc >= (startDate.HasValue ? startDate.Value.ToUniversalTime() : new DateTime(1900, 1, 1))
                    && r.CreatedUtc <= (endDate.HasValue ? endDate.Value.ToUniversalTime() : DateTime.UtcNow))
                .List()
                .ToList();
            var productIds = orders
                .SelectMany(order => order
                    .Items
                    .Select(item => item.ProductId))
                .Distinct()
                .ToList();
            var productTitles = _contentManager
                .GetMany<TitlePart>(productIds, VersionOptions.Published, QueryHints.Empty)
                .ToDictionary(
                    titlePart => titlePart.ContentItem.Id,
                    titlePart => titlePart.Title
                );

            using (var resultStream = new MemoryStream()) {
                using (var writer = new StreamWriter(resultStream, Encoding.UTF8)) {
                    writer.WriteLine(
                        T("ID").Text + "," +
                        T("Email").Text + "," +
                        T("Country").Text + "," +
                        T("Zip").Text + "," +
                        T("Date").Text + "," +
                        T("SubTotal").Text + "," +
                        T("Taxes").Text + "," +
                        T("Shipping").Text + "," +
                        T("Total").Text + "," +
                        String.Join(",", productIds
                            .Select(id => "\"" + productTitles[id] + " Qty\"")) + "," +
                        String.Join(",", productIds
                            .Select(id => "\"" + productTitles[id] + " Amt\"")));
                    foreach (var order in orders) {
                        var orderDateMaybe = order.As<CommonPart>().CreatedUtc;
                        var orderDate = orderDateMaybe.HasValue ? orderDateMaybe.Value.ToShortDateString() : "";
                        var quantities = order.Items
                            .GroupBy(item => item.ProductId)
                            .Select(group => new {
                                ProductId = group.Key,
                                Quantity = group.Sum(item => item.Quantity),
                                Total = group.Sum(item => (item.Quantity * item.Price) + item.LinePriceAdjustment)
                            })
                            .ToDictionary(
                                item => item.ProductId,
                                item => item
                            );
                        writer.WriteLine(
                            order.Id + "," +
                            "\"" + order.CustomerEmail + "\"," +
                            "\"" + order.ShippingAddress.Country + "\"," +
                            "\"" + order.ShippingAddress.PostalCode + "\"," +
                            orderDate + "," +
                            order.SubTotal.ToString("F") + "," +
                            order.Taxes.Amount.ToString("F") + "," +
                            order.ShippingOption.Price.ToString("F") + "," +
                            order.Total.ToString("F") + "," +
                            String.Join(",", productIds.Select(
                            id => quantities.ContainsKey(id) ? quantities[id].Quantity : 0)) + "," +
                            String.Join(",", productIds.Select(
                            id => (quantities.ContainsKey(id) ? quantities[id].Total : 0).ToString("F"))));
                    }
                }
                return new FileContentResult(resultStream.ToArray(), "text/csv") {
                    FileDownloadName = "Orders.csv"
                };
            }
        }
    }
}
