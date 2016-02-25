using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NuClear.River.Common.Metadata.Model
{
    /// <summary>
    /// Расширяемый набор функционала, использующего идентифицируемость сущностей.
    /// </summary>
    public static class IdentityExtensions
    {
        public static Expression<Func<T, bool>> Create<T, TKey>(this IIdentityProvider<TKey> identityProvider, TKey id)
            where T : IIdentifiable<TKey>
        {
            return CreateExpression<T, TKey>(identityProvider, new[] { id });
        }

        public static Expression<Func<T, bool>> Create<T, TKey>(this IIdentityProvider<TKey> identityProvider, IEnumerable<TKey> ids)
            where T : IIdentifiable<TKey>
        {
            return CreateExpression<T, TKey>(identityProvider, ids ?? Enumerable.Empty<TKey>());
        }

        public static TKey GetId<T, TKey>(this IIdentityProvider<TKey> identityProvider, T instance)
            where T : IIdentifiable<TKey>
        {
            var func = identityProvider.ExtractIdentity<T>().Compile();
            return func.Invoke(instance);
        }

        private static Expression<Func<TEntity, bool>> CreateExpression<TEntity, TKey>(IIdentityProvider<TKey> identity, IEnumerable<TKey> keys)
            where TEntity : IIdentifiable<TKey>
        {
            var expression = identity.ExtractIdentity<TEntity>();
            var containsMethod = GetMethodInfo<TKey>(Enumerable.Contains);
            return Expression.Lambda<Func<TEntity, bool>>(Expression.Call(null, containsMethod, Expression.Constant(keys), expression.Body),
                                                          expression.Parameters);
        }

        private static MethodInfo GetMethodInfo<TSource>(Func<IEnumerable<TSource>, TSource, bool> contains)
        {
            return contains.GetMethodInfo();
        }
    }
}