using System;
using System.Linq.Expressions;

using NuClear.Storage.API.Specifications;

namespace NuClear.Querying.Storage
{
    public static class Specs
    {
        public static class Find
        {
            public static FindSpecification<T> ById<T>(long id)
            {
                var propertyInfo = typeof(T).GetProperty("Id");
                if (propertyInfo == null || propertyInfo.PropertyType != typeof(long))
                {
                    throw new ArgumentException();
                }

                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, propertyInfo);
                var constant = ParametrizeConstant(id, typeof(long));
                var equals = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

                return new FindSpecification<T>(lambda);
            }

            // linq constant parametrization required for better sql server performance
            private static Expression ParametrizeConstant(object value, Type type)
            {
                var parameter = Activator.CreateInstance(typeof(LinqParameter<>).MakeGenericType(type), value);

                return Expression.Field(Expression.Constant(parameter), "Value");
            }

            private sealed class LinqParameter<T>
            {
                // ReSharper disable once MemberCanBePrivate.Local
                public readonly T Value;

                public LinqParameter(T value)
                {
                    Value = value;
                }
            }
        }
    }
}