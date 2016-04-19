using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Equality;

namespace NuClear.Replication.Core
{
    public sealed class EqualityComparerFactory : IEqualityComparerFactory
    {
        private readonly IObjectPropertyProvider _propertyProvider;
        private readonly IDictionary<Type, object> _identityComparerCache;
        private readonly IDictionary<Type, object> _completeComparerCache;

        public EqualityComparerFactory(IObjectPropertyProvider propertyProvider)
        {
            _propertyProvider = propertyProvider;
            _identityComparerCache = new Dictionary<Type, object>();
            _completeComparerCache = new Dictionary<Type, object>();
        }

        public IEqualityComparer<T> CreateIdentityComparer<T>()
        {
            object comparer;
            if (!_identityComparerCache.TryGetValue(typeof(T), out comparer))
            {
                var properties = _propertyProvider.GetPrimaryKeyProperties<T>();
                var equality = CreateEqualityFunction<T>(properties);
                var hashCode = CreateHashCodeFunction<T>(properties);
                _identityComparerCache[typeof(T)] = comparer = new EqualityComparerWrapper<T>(equality, hashCode);
            }

            return (IEqualityComparer<T>)comparer;
        }

        public IEqualityComparer<T> CreateCompleteComparer<T>()
        {
            object comparer;
            if (!_completeComparerCache.TryGetValue(typeof(T), out comparer))
            {
                var properties = _propertyProvider.GetProperties<T>();
                var equality = CreateEqualityFunction<T>(properties);
                var hashCode = CreateHashCodeFunction<T>(properties);
                _completeComparerCache[typeof(T)] = comparer = new EqualityComparerWrapper<T>(equality, hashCode);
            }

            return (IEqualityComparer<T>)comparer;
        }

        private Func<T, int> CreateHashCodeFunction<T>(IReadOnlyCollection<PropertyInfo> properties)
        {
            // T x => ((((x.P1.GetHashCode()) * 397) ^ x.P2.GetHashCode()) * 397) ^ x.P3.GetHashCode()

            var parameter = Expression.Parameter(typeof(T));
            var constPrimeNumber = Expression.Constant(397);

            var hashCodeMethod = typeof(object).GetRuntimeMethod("GetHashCode", new Type[0]);
            var hashCode = properties.Aggregate(
                (Expression)Expression.Constant(0),
                (acc, property) =>
                {
                    var propertyAccess = Expression.Property(parameter, property);
                    var propertyHashCode = property.PropertyType.GetTypeInfo().IsValueType
                                               ? (Expression)Expression.Call(propertyAccess, hashCodeMethod)
                                               : Expression.Condition(
                                                   Expression.ReferenceNotEqual(propertyAccess, Expression.Constant(null, property.PropertyType)),
                                                   Expression.Call(propertyAccess, hashCodeMethod),
                                                   Expression.Constant(0, typeof(int)));
                    return Expression.ExclusiveOr(Expression.Multiply(acc, constPrimeNumber), propertyHashCode);
                });

            return Expression.Lambda<Func<T, int>>(hashCode, parameter).Compile();
        }

        private Func<T, T, bool> CreateEqualityFunction<T>(IReadOnlyCollection<PropertyInfo> properties)
        {
            // (T x, T y) => x.Property1 == y.Property1 && x.Property2 == y.Property2 && ... && x.PropertyN == y.PropertyN

            var left = Expression.Parameter(typeof(T));
            var right = Expression.Parameter(typeof(T));
            var compare = properties.Select(p => Expression.Equal(Expression.Property(left, p), Expression.Property(right, p)))
                                    .Aggregate((Expression)Expression.Constant(true), Expression.And);

            return Expression.Lambda<Func<T, T, bool>>(compare, left, right).Compile();
        }
    }
}
