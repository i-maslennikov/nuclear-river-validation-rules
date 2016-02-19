using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NuClear.AdvancedSearch.Common.Metadata.Model
{
    /// <summary>
    /// Расширяемый набор функционала, использующего идентифицируемость сущностей.
    /// </summary>
    public static class IdentityExtensions
    {
        public static Expression<Func<T, bool>> Create<T>(this DefaultIdentity identity, long id)
            where T : IIdentifiable<DefaultIdentity, long>
        {
            return CreateExpression<T, DefaultIdentity, long>(identity, new[] { id });
        }

        public static Expression<Func<T, bool>> Create<T>(this DefaultIdentity identity, IEnumerable<long> ids)
            where T : IIdentifiable<DefaultIdentity, long>
        {
            return CreateExpression<T, DefaultIdentity, long>(identity, ids ?? Enumerable.Empty<long>());
        }

        public static long GetId<T>(this DefaultIdentity identity, T instance)
            where T : IIdentifiable<DefaultIdentity, long>
        {
            var func = identity.ExtractIdentity<T>().Compile();
            return func.Invoke(instance);
        }

        private static Expression<Func<TEntity, bool>> CreateExpression<TEntity, TIdentity, TKey>(TIdentity identity, IEnumerable<TKey> keys)
            where TEntity : IIdentifiable<TIdentity, TKey>
            where TIdentity : IIdentity<TKey>, new()
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