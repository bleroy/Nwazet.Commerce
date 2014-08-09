using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Commerce.Models.Reporting;
using Nwazet.Commerce.Permissions;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.Core.Common.ViewModels;
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
        private readonly IDateServices _dateServices;

        public ReportController(
            IEnumerable<ICommerceReport> reports,
            IOrchardServices orchardServices,
            IDateServices dateServices
            ) {
            _reports = reports;
            _orchardServices = orchardServices;
            _dateServices = dateServices;
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
                    return Report(report, TimePeriod.Day.BeginningDate(now), now, TimePeriod.Hour, preset);
                case "thisweek":
                    return Report(report, TimePeriod.Week.BeginningDate(now), now, TimePeriod.Day, preset);
                case "currentmonth":
                    return Report(report, TimePeriod.Month.BeginningDate(now), now, TimePeriod.Day, preset);
                case "lastfiveyears":
                    return Report(report, TimePeriod.Year.BeginningDate(now).AddYears(-4), now, TimePeriod.Year, preset);
                case "yeartodate":
                    return Report(report, TimePeriod.Year.BeginningDate(now), now, TimePeriod.Month, preset);
            }
            if (startDate == null || endDate == null) {
                return Report(report, TimePeriod.Year.BeginningDate(now), now, TimePeriod.Month, preset);
            }
            var parsedGranularity = TimePeriod.Parse(granularity);
            return Report(report, startDate.Value, endDate.Value, parsedGranularity);
        }

        private ActionResult Report(string report, DateTime startDate, DateTime endDate, TimePeriod granularity, string preset = null) {
            if (!_orchardServices.Authorizer.Authorize(ReportPermissions.GenerateReports, null, T("Cannot generate reports"))) {
                return new HttpUnauthorizedResult();
            }
            var reportService = _reports.FirstOrDefault(r => r.Name == report);
            if (reportService == null) {
                return HttpNotFound(T("Report {0} not found", report).Text);
            }
            var model = new ReportDataViewModel {
                Name = reportService.Name,
                Description = reportService.Description,
                DescriptionColumnHeader = reportService.DescriptionColumnHeader,
                ValueColumnHeader = reportService.ValueColumnHeader,
                ChartType = reportService.ChartType,
                DataPoints = reportService.GetData(startDate, endDate, granularity),
                StartDateEditor = new DateTimeEditor {
                    Date = _dateServices.ConvertToLocalDateString(startDate.ToUniversalTime()),
                    ShowDate = true,
                    ShowTime = false
                },
                EndDateEditor = new DateTimeEditor {
                    Date = _dateServices.ConvertToLocalDateString(endDate.ToUniversalTime()),
                    ShowDate = true,
                    ShowTime = false
                },
                Granularity = granularity
            };
            return View("Detail", model);
        }
    }
}
