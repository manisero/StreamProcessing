using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DataProcessing.Utils
{
    public static class PropertyInfoUtils
    {
        public static Func<TObject, TResult> CreateGetter<TObject, TResult>(
            this PropertyInfo property)
        {
            var parameter = Expression.Parameter(typeof(TObject));
            Expression propertyExpression = Expression.Property(parameter, property);

            if (property.PropertyType != typeof(TResult))
            {
                propertyExpression = property.PropertyType.IsPrimitive
                    ? Expression.Convert(propertyExpression, typeof(TResult))
                    : Expression.TypeAs(propertyExpression, typeof(TResult));
            }

            return Expression.Lambda<Func<TObject, TResult>>(propertyExpression, parameter).Compile();
        }
    }
}
