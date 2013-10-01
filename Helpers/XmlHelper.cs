using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace Nwazet.Commerce.Helpers {
    public static class XmlHelper {
        /// <summary>
        /// Like Add, but chainable.
        /// </summary>
        /// <param name="el">The parent element.</param>
        /// <param name="children">The elements to add.</param>
        /// <returns>Itself</returns>
        public static XElement AddEl(this XElement el, params XElement[] children) {
            el.Add(children.Cast<object>());
            return el;
        }

        /// <summary>
        /// Gets the string value of an attribute, and null if the attribute doesn't exist.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The string value of the attribute if it exists, null otherwise.</returns>
        public static string Attr(this XElement el, string name) {
            var attr = el.Attribute(name);
            return attr == null ? null : attr.Value;
        }

        /// <summary>
        /// Sets an attribute value. This is chainable.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="el">The element.</param>
        /// <param name="name">The attribute name.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>Itself</returns>
        public static XElement Attr<T>(this XElement el, string name, T value) {
            el.SetAttributeValue(name, value);
            return el;
        }

        /// <summary>
        /// Returns the text contents of a child element.
        /// </summary>
        /// <param name="el">The parent element.</param>
        /// <param name="name">The name of the child element.</param>
        /// <returns>The text for the child element, and null if it doesn't exist.</returns>
        public static string El(this XElement el, string name) {
            var childElement = el.Element(name);
            return childElement == null ? null : childElement.Value;
        }

        /// <summary>
        /// Creates and sets the value of a child element. This is chainable.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="el">The parent element.</param>
        /// <param name="name">The name of the child element.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>Itself</returns>
        public static XElement El<T>(this XElement el, string name, T value) {
            el.SetElementValue(name, value);
            return el;
        }

        /// <summary>
        /// Sets a property value from an attribute of the same name.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <typeparam name="TProperty">The type of the target property</typeparam>
        /// <param name="el">The element.</param>
        /// <param name="target">The target object.</param>
        /// <param name="targetExpression">The property expression.</param>
        /// <returns>Itself</returns>
        public static XElement FromAttr<TTarget, TProperty>(this XElement el, TTarget target,
            Expression<Func<TTarget, TProperty>> targetExpression) {

            var propertyInfo = GetPropertyInfo(targetExpression);
            var name = propertyInfo.Name;
            var attr = el.Attribute(name);

            if (attr == null) return el;
            if (typeof (TProperty) == typeof (string)) {
                propertyInfo.SetValue(target, (string) attr, null);
            }
            else if (attr.Value == "null") {
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

        /// <summary>
        /// Sets an attribute with the value of a property of the same name.
        /// </summary>
        /// <typeparam name="TTarget">The type of the object.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="el">The element.</param>
        /// <param name="target">The object.</param>
        /// <param name="targetExpression">The property expression.</param>
        /// <returns>Itself</returns>
        public static XElement ToAttr<TTarget, TProperty>(this XElement el, TTarget target,
            Expression<Func<TTarget, TProperty>> targetExpression) {
            var propertyInfo = GetPropertyInfo(targetExpression);
            var name = propertyInfo.Name;
            var val = propertyInfo.GetValue(target, null);

            if (typeof (TProperty) == typeof (string)) {
                el.Attr(name, (string) val);
            }
            else if (val == null) {
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

        private static PropertyInfo GetPropertyInfo<TContext, TProperty>(Expression<Func<TContext, TProperty>> expression) {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("Expression is not a member expression.");
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null) throw new InvalidOperationException("Expression is not for a property.");
            return propertyInfo;
        }

        /// <summary>
        /// Gives context to an XElement, enabling chained property operations.
        /// </summary>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <param name="el">The element.</param>
        /// <param name="context">The context.</param>
        /// <returns>The element with context.</returns>
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

            public static implicit operator XElement(XElementWithContext<TContext> elementWithContext) {
                return elementWithContext.Element;
            }

            /// <summary>
            /// Replaces the current context with a new one, enabling chained action on different objects.
            /// </summary>
            /// <typeparam name="TNewContext">The type of the new context.</typeparam>
            /// <param name="context">The new context.</param>
            /// <returns>A new XElementWithContext, that has the new context.</returns>
            public XElementWithContext<TNewContext> With<TNewContext>(TNewContext context) {
                return new XElementWithContext<TNewContext>(Element, context);
            }

            /// <summary>
            /// Sets the value of a context property as an attribute of the same name on the element.
            /// </summary>
            /// <typeparam name="TProperty">The type of the property.</typeparam>
            /// <param name="targetExpression">The property expression.</param>
            /// <returns>Itself</returns>
            public XElementWithContext<TContext> ToAttr<TProperty>(
                Expression<Func<TContext, TProperty>> targetExpression) {
                Element.ToAttr(Context, targetExpression);
                return this;
            }

            /// <summary>
            /// Gets an attribute on the element and sets the property of the same name on the context with its value.
            /// </summary>
            /// <typeparam name="TProperty">The type of the property.</typeparam>
            /// <param name="targetExpression">The property expression.</param>
            /// <returns>Itself</returns>
            public XElementWithContext<TContext> FromAttr<TProperty>(
                Expression<Func<TContext, TProperty>> targetExpression) {
                Element.FromAttr(Context, targetExpression);
                return this;
            }

            /// <summary>
            /// Evaluates an attribute from an expression.
            /// It's a nice strongly-typed way to read attributes.
            /// Cast the result if necessary
            /// (it would be great if C# could let us do the cast automatically
            /// but unfortunately it doesn't).
            /// </summary>
            /// <typeparam name="TProperty">The type of the property.</typeparam>
            /// <param name="expression">The property expression.</param>
            /// <returns>The attribute, ready to be cast.</returns>
            public XAttribute Attr<TProperty>(Expression<Func<TContext, TProperty>> expression) {
                var propertyInfo = GetPropertyInfo(expression);
                var name = propertyInfo.Name;
                return Element.Attribute(name);
            }
        }
    }
}
