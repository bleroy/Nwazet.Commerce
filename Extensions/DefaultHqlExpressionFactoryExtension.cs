using System;
using Orchard.ContentManagement;

namespace Nwazet.Commerce.Extensions {
    public static class DefaultHqlExpressionFactoryExtension {
        public static void InsensitiveLikeSpecificAlias(this IHqlExpressionFactory hqlExpressionFactory, string alias, string propertyName, string value, HqlMatchMode matchMode) {
            var aux = (hqlExpressionFactory as DefaultHqlExpressionFactory);
            aux.InsensitiveLike(propertyName, value, matchMode);
            var criterion = GetCriterion(alias, propertyName, value, matchMode);
            var property = typeof(DefaultHqlExpressionFactory).GetProperty("Criterion");
            property.SetValue(aux, criterion);
        }
        private static IHqlCriterion GetCriterion(string alias, string propertyName, string value, HqlMatchMode matchMode) {
            Func<string, string> processAlias = s => alias + s.Substring(s.IndexOf("."));
            switch (matchMode) {
                case HqlMatchMode.Start:
                    value = "'" + HqlRestrictions.FormatValue(value, false) + "%'";
                    break;
                case HqlMatchMode.Exact:
                    value = "'" + HqlRestrictions.FormatValue(value, false) + "'";
                    break;
                case HqlMatchMode.Anywhere:
                    value = "'%" + HqlRestrictions.FormatValue(value, false) + "%'";
                    break;
                case HqlMatchMode.End:
                    value = "'%" + HqlRestrictions.FormatValue(value, false) + "'";
                    break;
            }
            return new BinaryExpression("like", propertyName, value, processAlias);
        }
        public static void InSubquery(this IHqlExpressionFactory hqlExpressionFactory, string propertyName, string subquery) {
            var aux = (hqlExpressionFactory as DefaultHqlExpressionFactory);
            var crit = InSubquery(propertyName, subquery);
            var property = typeof(DefaultHqlExpressionFactory).GetProperty("Criterion");
            property.SetValue(aux, crit);
        }
        private static IHqlCriterion InSubquery(string propertyName, string subquery) {
            if (string.IsNullOrWhiteSpace(subquery)) {
                throw new ArgumentException("Subquery can't be empty", "subquery");
            }
            return new BinaryExpression("in", propertyName, "(" + subquery + ")");
        }
    }
}