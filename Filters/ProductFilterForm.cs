using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Forms.Services;

namespace Nwazet.Commerce.Filters {
    public class ProductFilterForm : IFormProvider {
        public Localizer T { get; set; }
        protected dynamic Shape { get; set; }

        public ProductFilterForm(IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }
        public void Describe(dynamic context) {
            Func<IShapeFactory, dynamic> form =
                shape => {

                    var f = Shape.Form(
                        Id: "FavoriteCultureForm",
                        _FavoriteCulture: Shape.TextBox(
                            Id: "FavoriteCulture", Name: "FavoriteCulture",
                            Title: T("Favorite Culture"),
                            Description: T("The culture code (e.g. en-US) or the culture id to filter for."),
                            Classes: new[] { "text", "tokenized" }
                            )
                            );
                    return f;
                };
            context.Form("FavoriteCultureForm", form);
        }
    }
}

