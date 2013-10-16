using System;
using System.Linq.Expressions;
using Nwazet.Commerce.Helpers;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Models {
    // These base classes won't be necessary any more once
    // their methods are integrated into core's ContentPart and ContentPart<TRecord>.
    public class InfosetContentPart<TRecord> : ContentPart<TRecord> {

        protected TProperty Get<TProperty>(Expression<Func<TRecord, TProperty>> targetExpression) {
            return InfosetHelper.Get(this, targetExpression);
        }

        protected TProperty Get<TProperty>(
            Expression<Func<TRecord, TProperty>> targetExpression,
            Func<TRecord, TProperty> defaultExpression) {

            return InfosetHelper.Get(this, targetExpression, defaultExpression);
        }

        protected InfosetContentPart<TRecord> Set<TProperty>(
            Expression<Func<TRecord, TProperty>> targetExpression,
            TProperty value) {

            InfosetHelper.Set(this, targetExpression, value);
            return this;
        }
    }
}
