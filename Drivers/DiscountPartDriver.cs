using System;
using System.Globalization;
using System.Linq;
using Nwazet.Commerce.Models;
using Nwazet.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Roles.Services;

namespace Nwazet.Commerce.Drivers {
    [OrchardFeature("Nwazet.Promotions")]
    public class DiscountPartDriver : ContentPartDriver<DiscountPart> {
        private readonly IRoleService _roleService;

        public DiscountPartDriver(
            IOrchardServices services,
            IRoleService roleService) {

            _roleService = roleService;
            Services = services;
        }

        public IOrchardServices Services { get; set; }

        protected override string Prefix { get { return "NwazetCommerceDiscount"; } }

        protected override DriverResult Display(
            DiscountPart part, string displayType, dynamic shapeHelper) {
            // The discount part should never appear on the front-end.
            // Nwazet.Commerce will pick up any promotions from its own interfaces instead.
            return null;
        }

        //GET
        protected override DriverResult Editor(DiscountPart part, dynamic shapeHelper) {
            var currentCulture = Services.WorkContext.CurrentCulture;
            var cultureInfo = CultureInfo.GetCultureInfo(currentCulture);
            var currentTimeZone = Services.WorkContext.CurrentTimeZone;
            DateTime? localStartDate = part.StartDate == null ?
                (DateTime?)null :
                TimeZoneInfo.ConvertTimeFromUtc((DateTime)part.StartDate, currentTimeZone);
            DateTime? localEndDate = part.EndDate == null ?
                (DateTime?)null :
                TimeZoneInfo.ConvertTimeFromUtc((DateTime)part.EndDate, currentTimeZone);
            return ContentShape(
                "Parts_Discount_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Discount",
                    Prefix: Prefix,
                    Model: new DiscountEditorViewModel {
                        Name = part.Name,
                        Discount = part.Record.Discount,
                        StartDate = localStartDate.HasValue ? ((DateTime)localStartDate).ToString("d", cultureInfo) : String.Empty,
                        StartTime = localStartDate.HasValue ? ((DateTime)localStartDate).ToString("t", cultureInfo) : String.Empty,
                        EndDate = localEndDate.HasValue ? ((DateTime)localEndDate).ToString("d", cultureInfo) : String.Empty,
                        EndTime = localEndDate.HasValue ? ((DateTime)localEndDate).ToString("t", cultureInfo) : String.Empty,
                        StartQuantity = part.StartQuantity,
                        EndQuantity = part.EndQuantity,
                        Roles = _roleService.GetRoles().Select(r => r.Name).ToList(),
                        DiscountRoles = part.Roles.ToArray(),
                        Pattern = part.Pattern,
                        ExclusionPattern = part.ExclusionPattern,
                        Comment = part.Comment
                    }));
        }

        //POST
        protected override DriverResult Editor(DiscountPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new DiscountEditorViewModel();
            if (updater.TryUpdateModel(model, Prefix, null, null)) {
                part.Name = model.Name;
                string discountString = model.Discount;
                decimal percent;
                decimal discount;
                if (!String.IsNullOrEmpty(discountString)) {
                    if (discountString.Trim().EndsWith("%")) {
                        if (decimal.TryParse(discountString.Substring(0, discountString.Length - 1), out percent)) {
                            part.DiscountPercent = percent;
                        }
                    }
                    else {
                        if (decimal.TryParse(discountString, out discount)) {
                            part.Discount = discount;
                        }
                    }
                }
                var currentCulture = Services.WorkContext.CurrentCulture;
                var cultureInfo = CultureInfo.GetCultureInfo(currentCulture);
                var timeZone = Services.WorkContext.CurrentTimeZone;
                if (!String.IsNullOrWhiteSpace(model.StartDate)) {
                    DateTime startDate;
                    var parseStartDateTime = String.Concat(model.StartDate, " ", model.StartTime);
                    if (DateTime.TryParse(parseStartDateTime, cultureInfo, DateTimeStyles.None, out startDate)) {
                        part.StartDate = TimeZoneInfo.ConvertTimeToUtc(startDate, timeZone);
                    }
                }
                else {
                    part.StartDate = null;
                }
                if (!String.IsNullOrWhiteSpace(model.EndDate)) {
                    DateTime endDate;
                    var parseEndDateTime = String.Concat(model.EndDate, " ", model.EndTime);
                    if (DateTime.TryParse(parseEndDateTime, cultureInfo, DateTimeStyles.None, out endDate)) {
                        part.EndDate = TimeZoneInfo.ConvertTimeToUtc(endDate, timeZone);
                    }
                }
                else {
                    part.EndDate = null;
                }
                part.StartQuantity = model.StartQuantity;
                part.EndQuantity = model.EndQuantity;
                part.Roles = model.DiscountRoles;
                part.Pattern = model.Pattern;
                part.ExclusionPattern = model.ExclusionPattern;
                part.Comment = model.Comment;
            }
            return Editor(part, shapeHelper);
        }

        protected override void Importing(DiscountPart part, ImportContentContext context) {
            var el = context.Data.Element(typeof(DiscountPart).Name);
            if (el == null) return;
            el.With(part)
              .FromAttr(p => p.Name)
              .FromAttr(p => p.StartDate)
              .FromAttr(p => p.EndDate)
              .FromAttr(p => p.StartQuantity)
              .FromAttr(p => p.EndQuantity)
              .FromAttr(p => p.Pattern)
              .FromAttr(p => p.ExclusionPattern)
              .FromAttr(p => p.Comment)
              .With(part.Record)
              .FromAttr(r => r.Discount)
              .FromAttr(r => r.Roles);
        }

        protected override void Exporting(DiscountPart part, ExportContentContext context) {
            context.Element(typeof (DiscountPart).Name)
                   .With(part)
                   .ToAttr(p => p.Name)
                   .ToAttr(p => p.StartDate)
                   .ToAttr(p => p.EndDate)
                   .ToAttr(p => p.StartQuantity)
                   .ToAttr(p => p.EndQuantity)
                   .ToAttr(p => p.Pattern)
                   .ToAttr(p => p.ExclusionPattern)
                   .ToAttr(p => p.Comment)
                   .With(part.Record)
                   .ToAttr(r => r.Discount)
                   .ToAttr(r => r.Roles);
        }
    }
}
