using System;
using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.Environment;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Projections.FilterEditors.Forms;
using Orchard.UI.Resources;

namespace Nwazet.Commerce.Filters {
    public class ProductWeightFilterForm : IFormProvider {
        public Localizer T { get; set; }
        protected dynamic Shape { get; set; }
        private readonly Work<IResourceManager> _resourceManager;
        public ProductWeightFilterForm(IShapeFactory shapeFactory, Work<IResourceManager> resourceManager) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
            _resourceManager = resourceManager;
        }
        public void Describe(DescribeContext context) {
            Func<IShapeFactory, dynamic> form =
                shape => {
                    var f = Shape.Form(
                        Id: "ProductWeightFilterForm",
                       _Operator: Shape.SelectList(
                            Id: "operator", Name: "Operator",
                            Title: T("Operator"),
                            Size: 1,
                            Multiple: false
                        ),
                       _FieldSetSingle: Shape.FieldSet(
                           Id: "fieldset-single",
                           _Value: Shape.TextBox(
                               Id: "value", Name: "Value",
                               Title: T("Value"),
                               Classes: new[] { "tokenized" }
                               )
                           ),
                       _FieldSetMin: Shape.FieldSet(
                           Id: "fieldset-min",
                           _Min: Shape.TextBox(
                               Id: "min", Name: "Min",
                               Title: T("Min"),
                               Classes: new[] { "tokenized" }
                               )
                           ),
                       _FieldSetMax: Shape.FieldSet(
                           Id: "fieldset-max",
                           _Max: Shape.TextBox(
                               Id: "max", Name: "Max",
                               Title: T("Max"),
                               Classes: new[] { "tokenized" }
                               )
                           )
                   );
                    _resourceManager.Value.Require("script", "jQuery");
                    _resourceManager.Value.Include("script", "~/Modules/Orchard.Projections/Scripts/numeric-editor-filter.js", "~/Modules/Orchard.Projections/Scripts/numeric-editor-filter.js");

                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.LessThan), Text = T("Is less than").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.LessThanEquals), Text = T("Is less than or equal to").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.Equals), Text = T("Is equal to").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.NotEquals), Text = T("Is not equal to").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.GreaterThanEquals), Text = T("Is greater than or equal to").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.GreaterThan), Text = T("Is greater than").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.Between), Text = T("Is between").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(NumericOperator.NotBetween), Text = T("Is not between").Text });
                    return f;
                };
            context.Form("ProductWeightFilterForm", form);
        }
    }
}

