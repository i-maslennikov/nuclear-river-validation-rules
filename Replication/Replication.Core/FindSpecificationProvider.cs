using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core
{
    internal sealed class FindSpecificationProvider<T, TKey> : IFindSpecificationProvider<T, TKey>
        where T : IIdentifiable<TKey>
    {
        private readonly IIdentityProvider<TKey> _identityProvider;

        public FindSpecificationProvider(IIdentityProvider<TKey> identityProvider)
        {
            _identityProvider = identityProvider;
        }

        public FindSpecification<T> Create(IEnumerable<TKey> keys)
        {
            return new FindSpecification<T>(CreateExpression(_identityProvider, keys));
        }

        private static Expression<Func<T, bool>> CreateExpression(IIdentityProvider<TKey> identity, IEnumerable<TKey> keys)
        {
            var expression = identity.Get<T>();
            var containsMethod = GetMethodInfo<TKey>(Enumerable.Contains);
            return Expression.Lambda<Func<T, bool>>(Expression.Call(null, containsMethod, Expression.Constant(keys), expression.Body),
                                                    expression.Parameters);
        }

        private static MethodInfo GetMethodInfo<TSource>(Func<IEnumerable<TSource>, TSource, bool> contains)
        {
            return contains.GetMethodInfo();
        }
    }
}