using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NuClear.CustomerIntelligence.Replication.Tests
{
    public static class QueryableExtensions
    {
        public static IEnumerable<T> By<T>(this IQueryable<T> queryable, Expression<Func<T, long>> idProvider, long id)
        {
            var where = GetMethodInfo<long>(Queryable.Where).GetGenericMethodDefinition().MakeGenericMethod(typeof(T));

            var parameter = Expression.Parameter(typeof(long));
            var whereArgument = Expression.Lambda<Func<T, bool>>(Expression.Equal(idProvider.Body, parameter), idProvider.Parameters);
            var whereCall = Expression.Call(null, where, Expression.Constant(queryable), whereArgument);
            var lambda = Expression.Lambda<Func<long, IEnumerable<T>>>(whereCall, parameter).Compile();

            return lambda(id);
        }

        public static IEnumerable<T> By<T>(this IQueryable<T> queryable, Expression<Func<T, long>> idProvider, IEnumerable<long> ids)
        {
            var where = GetMethodInfo<long>(Queryable.Where).GetGenericMethodDefinition().MakeGenericMethod(typeof(T));
            var contains = GetMethodInfo<long>(Enumerable.Contains);
            var parameter = Expression.Parameter(typeof(IEnumerable<long>));
            var whereArgument = Expression.Lambda<Func<T, bool>>(Expression.Call(null, contains, parameter, idProvider.Body), idProvider.Parameters);

            var whereCall = Expression.Call(null, where, Expression.Constant(queryable), whereArgument);
            var lambda = Expression.Lambda<Func<IEnumerable<long>, IEnumerable<T>>>(whereCall, parameter).Compile();

            return lambda(ids);
        }

        private static MethodInfo GetMethodInfo<T>(Func<IQueryable<T>, Expression<Func<T, bool>>, IQueryable<T>> predicate)
        {
            return predicate.GetMethodInfo();
        }

        private static MethodInfo GetMethodInfo<TSource>(Func<IEnumerable<TSource>, TSource, bool> contains)
        {
            return contains.GetMethodInfo();
        }
    }
}