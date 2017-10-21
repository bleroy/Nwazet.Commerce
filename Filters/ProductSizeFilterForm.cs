
using System;
using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Projections.FilterEditors.Forms;

namespace Nwazet.Commerce.Filters {

    public class ProductSizeFilterForm : IFormProvider {
        public Localizer T { get; set; }
        protected dynamic Shape { get; set; }

        public ProductSizeFilterForm(IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }
        public void Describe(DescribeContext context) {
            Func<IShapeFactory, dynamic> form =
                shape => {
                    var f = Shape.Form(
                        Id: "ProductSizeFilterForm",
                        _ProductFilterForm: Shape.TextBox(
                            Id: "Value", Name: "Value",
                            Title: T("The Product Size"),
                            Description: T("Product Size."),
                            Classes: new[] { "text", "tokenized" }
                        ),
                        _Operator: Shape.SelectList(
                            Id: "operator", Name: "Operator",
                            Title: T("Operator"),
                            Size: 1,
                            Multiple: false
                        ));
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(StringOperator.Equals), Text = T("Is equal to").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(StringOperator.NotEquals), Text = T("Is not equal to").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(StringOperator.Contains), Text = T("Contains").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(StringOperator.ContainsAny), Text = T("Contains any word").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(StringOperator.ContainsAll), Text = T("Contains all words").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(StringOperator.Starts), Text = T("Starts with").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(StringOperator.NotStarts), Text = T("Does not start with").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(StringOperator.Ends), Text = T("Ends with").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(StringOperator.NotEnds), Text = T("Does not end with").Text });
                    f._Operator.Add(new SelectListItem { Value = Convert.ToString(StringOperator.NotContains), Text = T("Does not contain").Text });

                    return f;
                };
            context.Form("ProductSizeFilterForm", form);
        }
    }
}
