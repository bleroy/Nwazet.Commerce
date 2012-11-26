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
                        Comment = part.Comment
                    }));
        }

        //POST
        protected override DriverResult Editor(DiscountPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new DiscountEditorViewModel();
            if (updater.TryUpdateModel(model, Prefix, null, null)) {
                part.Name = model.Name;
                part.Record.Discount = model.Discount;
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
                part.Comment = model.Comment;
            }
            return Editor(part, shapeHelper);
        }

        protected override void Importing(DiscountPart part, ImportContentContext context) {
            var name = context.Attribute(part.PartDefinition.Name, "Name");
            if (!String.IsNullOrWhiteSpace(name)) {
                part.Name = name;
            }
            var discountString = context.Attribute(part.PartDefinition.Name, "Discount");
            if (!String.IsNullOrWhiteSpace(discountString)) {
                part.Record.Discount = discountString;
            }
            var startDateString = context.Attribute(part.PartDefinition.Name, "StartDate");
            if (String.IsNullOrWhiteSpace(startDateString)) {
                part.StartDate = null;
            }
            else {
                DateTime startDate;
                if (DateTime.TryParse(startDateString, out startDate)) {
                    part.StartDate = startDate;
                }
            }
            var endDateString = context.Attribute(part.PartDefinition.Name, "EndDate");
            if (String.IsNullOrWhiteSpace(endDateString)) {
                part.EndDate = null;
            }
            else {
                DateTime endDate;
                if (DateTime.TryParse(endDateString, out endDate)) {
                    part.EndDate = endDate;
                }
            }
            var startQuantityString = context.Attribute(part.PartDefinition.Name, "StartQuantity");
            if (String.IsNullOrWhiteSpace(startQuantityString)) {
                part.StartQuantity = null;
            }
            else {
                int startQuantity;
                if (int.TryParse(startQuantityString, out startQuantity)) {
                    part.StartQuantity = startQuantity;
                }
            }
            var endQuantityString = context.Attribute(part.PartDefinition.Name, "EndQuantity");
            if (String.IsNullOrWhiteSpace(endQuantityString)) {
                part.EndQuantity = null;
            }
            else {
                int endQuantity;
                if (int.TryParse(endQuantityString, out endQuantity)) {
                    part.EndQuantity = endQuantity;
                }
            }
            part.Record.Roles = context.Attribute(part.PartDefinition.Name, "Roles");
            part.Pattern = context.Attribute(part.PartDefinition.Name, "Pattern") ?? "";
            part.Comment = context.Attribute(part.PartDefinition.Name, "Comment") ?? "";
        }

        protected override void Exporting(DiscountPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Name", part.Name);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Discount", part.Record.Discount);
            if (part.StartDate != null) {
                context.Element(part.PartDefinition.Name).SetAttributeValue("StartDate", part.StartDate.ToString());
            }
            if (part.EndDate != null) {
                context.Element(part.PartDefinition.Name).SetAttributeValue("EndDate", part.EndDate.ToString());
            }
            if (part.StartQuantity != null) {
                context.Element(part.PartDefinition.Name).SetAttributeValue("StartQuantity", part.StartQuantity.ToString());
            }
            if (part.EndQuantity != null) {
                context.Element(part.PartDefinition.Name).SetAttributeValue("EndQuantity", part.EndQuantity.ToString());
            }
            if (!String.IsNullOrWhiteSpace(part.Record.Roles)) {
                context.Element(part.PartDefinition.Name).SetAttributeValue("Roles", part.Record.Roles);
            }
            if (!String.IsNullOrWhiteSpace(part.Pattern)) {
                context.Element(part.PartDefinition.Name).SetAttributeValue("Pattern", part.Pattern);
            }
            if (!String.IsNullOrWhiteSpace(part.Comment)) {
                context.Element(part.PartDefinition.Name).SetAttributeValue("Comment", part.Comment);
            }
        }
    }
}
