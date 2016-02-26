using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NuClear.Querying.Storage
{
    public static class QueryableExtensions
    {
        public static IQueryable<TK> SelectManyProperties<T, TK>(this IQueryable<T> query, string propertyName)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null || !typeof(IEnumerable<TK>).IsAssignableFrom(propertyInfo.PropertyType))
            {
                throw new ArgumentException();
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda<Func<T, IEnumerable<TK>>>(property, parameter);

            return query.SelectMany(lambda);
        }
    }
}