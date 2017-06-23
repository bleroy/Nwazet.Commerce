using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.FilterEditors.Forms;

namespace Nwazet.Commerce.Filters {
   public static class FilterHelper {
        public enum PropertyType { StringType, NumericType };
        public static Action<IHqlExpressionFactory> GetFilterPredicateNumeric(NumericOperator op, string property, string value, string min, string max) {
            decimal dmin, dmax;
            if (op == NumericOperator.Between || op == NumericOperator.NotBetween) {
                dmin = Decimal.Parse(Convert.ToString(min), CultureInfo.InvariantCulture);
                dmax = Decimal.Parse(Convert.ToString(max), CultureInfo.InvariantCulture);
            }
            else {
                dmin = dmax = Decimal.Parse(Convert.ToString(value), CultureInfo.InvariantCulture);
            }

            switch (op) {
                case NumericOperator.LessThan:
                    return x => x.Lt(property, dmax);
                case NumericOperator.LessThanEquals:
                    return x => x.Le(property, dmax);
                case NumericOperator.Equals:
                    if (dmin == dmax) {
                        return x => x.Eq(property, dmin);
                    }
                    return y => y.And(x => x.Ge(property, dmin), x => x.Le(property, dmax));
                case NumericOperator.NotEquals:
                    return dmin == dmax ? (Action<IHqlExpressionFactory>)(x => x.Not(y => y.Eq(property, dmin))) : (y => y.Or(x => x.Lt(property, dmin), x => x.Gt(property, dmax)));
                case NumericOperator.GreaterThan:
                    return x => x.Gt(property, dmin);
                case NumericOperator.GreaterThanEquals:
                    return x => x.Ge(property, dmin);
                case NumericOperator.Between:
                    return y => y.And(x => x.Ge(property, dmin), x => x.Le(property, dmax));
                case NumericOperator.NotBetween:
                    return y => y.Or(x => x.Lt(property, dmin), x => x.Gt(property, dmax));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static  Action<IHqlExpressionFactory> GetFilterPredicateString(StringOperator op, string property, string value) {
            switch (op) {
                case StringOperator.Equals:
                    return x => x.Eq(property, value);
                case StringOperator.NotEquals:
                    return x => x.Not(y => y.Eq(property, value));
                case StringOperator.Contains:
                    return x => x.Like(property, Convert.ToString(value), HqlMatchMode.Anywhere);
                case StringOperator.ContainsAny:
                    if (string.IsNullOrEmpty((string)value))
                        return x => x.Eq("Id", "0");
                    var values1 = Convert.ToString(value).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var predicates1 = values1.Skip(1).Select<string, Action<IHqlExpressionFactory>>(x => y => y.Like(property, x, HqlMatchMode.Anywhere)).ToArray();
                    return x => x.Disjunction(y => y.Like(property, values1[0], HqlMatchMode.Anywhere), predicates1);
                case StringOperator.ContainsAll:
                    var values2 = Convert.ToString(value).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var predicates2 = values2.Skip(1).Select<string, Action<IHqlExpressionFactory>>(x => y => y.Like(property, x, HqlMatchMode.Anywhere)).ToArray();
                    return x => x.Conjunction(y => y.Like(property, values2[0], HqlMatchMode.Anywhere), predicates2);
                case StringOperator.Starts:
                    return x => x.Like(property, Convert.ToString(value), HqlMatchMode.Start);
                case StringOperator.NotStarts:
                    return y => y.Not(x => x.Like(property, Convert.ToString(value), HqlMatchMode.Start));
                case StringOperator.Ends:
                    return x => x.Like(property, Convert.ToString(value), HqlMatchMode.End);
                case StringOperator.NotEnds:
                    return y => y.Not(x => x.Like(property, Convert.ToString(value), HqlMatchMode.End));
                case StringOperator.NotContains:
                    return y => y.Not(x => x.Like(property, Convert.ToString(value), HqlMatchMode.Anywhere));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static LocalizedString DisplayFilterNumeric(Localizer T,dynamic contextState,  string property) {
            string value = contextState.Value;
            string min = contextState.Min;
            string max = contextState.Max;
            var op = (NumericOperator)Enum.Parse(typeof(NumericOperator), Convert.ToString(contextState.Operator));

            switch (op) {
                case NumericOperator.LessThan:
                    return T("{0} is less than {1}", property, value);
                case NumericOperator.LessThanEquals:
                    return T("{0} is less than or equal to {1}", property, value);
                case NumericOperator.Equals:
                    return T("{0} equals {1}", property, value);
                case NumericOperator.NotEquals:
                    return T("{0} is not equal to {1}", property, value);
                case NumericOperator.GreaterThan:
                    return T("{0} is greater than {1}", property, value);
                case NumericOperator.GreaterThanEquals:
                    return T("{0} is greater than or equal to {1}", property, value);
                case NumericOperator.Between:
                    return T("{0} is between {1} and {2}", property, min, max);
                case NumericOperator.NotBetween:
                    return T("{0} is not between {1} and {2}", property, min, max);
            }
            return new LocalizedString(property);
        }

        public static LocalizedString DisplayFilterString(Localizer T, dynamic contextState, string property) {
            var op = (StringOperator)Enum.Parse(typeof(StringOperator), Convert.ToString(contextState.Operator));
            string value = Convert.ToString(contextState.Value);

            switch (op) {
                case StringOperator.Equals:
                    return T("{0} is equal to '{1}'", contextState, value);
                case StringOperator.NotEquals:
                    return T("{0} is not equal to '{1}'", property, value);
                case StringOperator.Contains:
                    return T("{0} contains '{1}'", property, value);
                case StringOperator.ContainsAny:
                    return T("{0} contains any of '{1}'", property, new LocalizedString(String.Join("', '", value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))));
                case StringOperator.ContainsAll:
                    return T("{0} contains all '{1}'", property, new LocalizedString(String.Join("', '", value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))));
                case StringOperator.Starts:
                    return T("{0} starts with '{1}'", property, value);
                case StringOperator.NotStarts:
                    return T("{0} does not start with '{1}'", property, value);
                case StringOperator.Ends:
                    return T("{0} ends with '{1}'", property, value);
                case StringOperator.NotEnds:
                    return T("{0} does not end with '{1}'", property, value);
                case StringOperator.NotContains:
                    return T("{0} does not contain '{1}'", property, value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
