using System;
using System.Linq.Expressions;
using System.Xml.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;

namespace Nwazet.Commerce.Helpers {
    public static class InfosetHelper {
        public static TProperty Get<TProperty>(this ContentPart contentPart, string name) {
            var infosetPart = contentPart.As<InfosetPart>();
            var el = infosetPart == null
                ? null
                : infosetPart.Infoset.Element.Element(contentPart.GetType().Name);
            return el == null ? default(TProperty) : el.Attr<TProperty>(name);
        }

        public static void Set<TProperty>(this ContentPart contentPart,
            string name, TProperty value) {

            var partName = contentPart.GetType().Name;

            var infosetPart = contentPart.As<InfosetPart>();
            var infoset = infosetPart.Infoset;
            var partElement = infoset.Element.Element(partName);
            if (partElement == null) {
                partElement = new XElement(partName);
                infoset.Element.Add(partElement);
            }
            partElement.Attr(name, value);
        }

        public static TProperty Get<TPart, TRecord, TProperty>(this TPart contentPart,
            Expression<Func<TRecord, TProperty>> targetExpression)
            where TPart : ContentPart<TRecord> {

            var getter = ReflectionHelper<TRecord>.GetGetter(targetExpression);
            return contentPart.Get(targetExpression, getter);
        }

        public static TProperty Get<TPart, TRecord, TProperty>(this TPart contentPart,
            Expression<Func<TRecord, TProperty>> targetExpression,
            Delegate defaultExpression)
            where TPart : ContentPart<TRecord> {

            var propertyInfo = ReflectionHelper<TRecord>.GetPropertyInfo(targetExpression);
            var name = propertyInfo.Name;

            var infosetPart = contentPart.As<InfosetPart>();
            var el = infosetPart == null
                ? null
                : infosetPart.Infoset.Element.Element(contentPart.GetType().Name);
            if (el == null || el.Attribute(name) == null) {
                // Property has never been stored. Get it from the default expression and store that.
                var defaultValue = defaultExpression == null
                    ? default(TProperty)
                    : (TProperty)defaultExpression.DynamicInvoke(contentPart.Record);
                contentPart.Set(name, defaultValue);
                return defaultValue;
            }
            return el.Attr<TProperty>(name);
        }

        public static void Set<TPart, TRecord, TProperty>(this TPart contentPart,
            Expression<Func<TRecord, TProperty>> targetExpression,
            TProperty value)
            where TPart : ContentPart<TRecord> {
            
            var propertyInfo = ReflectionHelper<TRecord>.GetPropertyInfo(targetExpression);
            var name = propertyInfo.Name;
            propertyInfo.SetValue(contentPart.Record, value);
            contentPart.Set(name, value);
        }
    }
}
