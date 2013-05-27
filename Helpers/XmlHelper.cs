using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace Nwazet.Commerce.Helpers {
    public static class XmlHelper {

        public static string Attr(this XElement el, string name) {
            var attr = el.Attribute(name);
            return attr == null ? null : attr.Value;
        }

        public static XElement Attr<T>(this XElement el, string name, T value) {
            el.SetAttributeValue(name, value);
            return el;
        }

        public static string El(this XElement el, string name) {
            var childElement = el.Element(name);
            return childElement == null ? null : childElement.Value;
        }

        public static XElement El<T>(this XElement el, string name, T value) {
            el.SetElementValue(name, value);
            return el;
        }

        public static XElement FromAttr<TTarget, TProperty>(this XElement el, TTarget target,
                                                            Expression<Func<TTarget, TProperty>> targetExpression) {
            var memberExpression = targetExpression.Body as MemberExpression;
            if (memberExpression == null) throw new InvalidOperationException("Expression is not a member expression.");
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null) throw new InvalidOperationException("Expression is not for a property.");
            var name = propertyInfo.Name;
            var attr = el.Attribute(name);
            if (attr == null) return el;
            if (typeof (TProperty) == typeof (string)) {
                propertyInfo.SetValue(target, (string) attr, null);
                return el;
            }
            if (attr.Value == "null") {
                propertyInfo.SetValue(target, null, null);
            }
            else if (typeof (TProperty) == typeof (int)) {
                propertyInfo.SetValue(target, (int) attr, null);
            }
            else if (typeof (TProperty) == typeof (bool)) {
                propertyInfo.SetValue(target, (bool) attr, null);
            }
            else if (typeof (TProperty) == typeof (DateTime)) {
                propertyInfo.SetValue(target, (DateTime) attr, null);
            }
            else if (typeof (TProperty) == typeof (double)) {
                propertyInfo.SetValue(target, (double) attr, null);
            }
            else if (typeof (TProperty) == typeof (float)) {
                propertyInfo.SetValue(target, (float) attr, null);
            }
            else if (typeof (TProperty) == typeof (decimal)) {
                propertyInfo.SetValue(target, (decimal) attr, null);
            }
            else if (typeof (TProperty) == typeof (int?)) {
                propertyInfo.SetValue(target, (int?) attr, null);
            }
            else if (typeof (TProperty) == typeof (bool?)) {
                propertyInfo.SetValue(target, (bool?) attr, null);
            }
            else if (typeof (TProperty) == typeof (DateTime?)) {
                propertyInfo.SetValue(target, (DateTime?) attr, null);
            }
            else if (typeof (TProperty) == typeof (double?)) {
                propertyInfo.SetValue(target, (double?) attr, null);
            }
            else if (typeof (TProperty) == typeof (float?)) {
                propertyInfo.SetValue(target, (float?) attr, null);
            }
            else if (typeof (TProperty) == typeof (decimal?)) {
                propertyInfo.SetValue(target, (decimal?) attr, null);
            }
            return el;
        }

        public static XElement ToAttr<TTarget, TProperty>(this XElement el, TTarget target,
                                                          Expression<Func<TTarget, TProperty>> targetExpression) {
            var memberExpression = targetExpression.Body as MemberExpression;
            if (memberExpression == null) throw new InvalidOperationException("Expression is not a member expression.");
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null) throw new InvalidOperationException("Expression is not for a property.");
            var name = propertyInfo.Name;
            var val = propertyInfo.GetValue(target, null);
            if (typeof (TProperty) == typeof (string)) {
                el.Attr(name, (string) val);
                return el;
            }
            if (val == null) {
                el.Attr(name, "null");
            }
            else if (typeof (TProperty) == typeof (int)) {
                el.Attr(name, (int) val);
            }
            else if (typeof (TProperty) == typeof (bool)) {
                el.Attr(name, (bool) val);
            }
            else if (typeof (TProperty) == typeof (DateTime)) {
                el.Attr(name, (DateTime) val);
            }
            else if (typeof (TProperty) == typeof (double)) {
                el.Attr(name, (double) val);
            }
            else if (typeof (TProperty) == typeof (float)) {
                el.Attr(name, (float) val);
            }
            else if (typeof (TProperty) == typeof (decimal)) {
                el.Attr(name, (decimal) val);
            }
            else if (typeof (TProperty) == typeof (int?)) {
                el.Attr(name, (int?) val);
            }
            else if (typeof (TProperty) == typeof (bool?)) {
                el.Attr(name, (bool?) val);
            }
            else if (typeof (TProperty) == typeof (DateTime?)) {
                el.Attr(name, (DateTime?) val);
            }
            else if (typeof (TProperty) == typeof (double?)) {
                el.Attr(name, (double?) val);
            }
            else if (typeof (TProperty) == typeof (float?)) {
                el.Attr(name, (float?) val);
            }
            else if (typeof (TProperty) == typeof (decimal?)) {
                el.Attr(name, (decimal?) val);
            }
            return el;
        }

        public static XElementWithContext<TContext> With<TContext>(this XElement el, TContext context) {
            return new XElementWithContext<TContext>(el, context);
        }

        public class XElementWithContext<TContext> {
            public XElementWithContext(XElement element, TContext context) {
                Element = element;
                Context = context;
            }

            public XElement Element { get; private set; }
            public TContext Context { get; private set; }

            public XElementWithContext<TNewContext> With<TNewContext>(TNewContext context) {
                return new XElementWithContext<TNewContext>(Element, context);
            }

            public XElementWithContext<TContext> ToAttr<TProperty>(
                Expression<Func<TContext, TProperty>> targetExpression) {
                Element.ToAttr(Context, targetExpression);
                return this;
            }

            public XElementWithContext<TContext> FromAttr<TProperty>(
                Expression<Func<TContext, TProperty>> targetExpression) {
                Element.FromAttr(Context, targetExpression);
                return this;
            }
        }
    }
}
