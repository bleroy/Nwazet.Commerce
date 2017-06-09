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
    }
}