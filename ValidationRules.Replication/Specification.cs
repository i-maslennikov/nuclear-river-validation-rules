using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.Replication
{
    public sealed class Specification<T>
    {
        private const int MsSqlBatchLimitation = 5000;

        public static FindSpecification<T> Create<TKey>(Expression<Func<T, TKey>> field, IReadOnlyCollection<TKey> allowedKeys)
        {
            return new Containment<TKey>(field, allowedKeys);
        }

        private sealed class Containment<TKey> : FindSpecification<T>, IBatchableSpecification<T>
        {
            private static readonly Func<IEnumerable<TKey>, TKey, bool> Contains = Enumerable.Contains;

            private readonly Expression<Func<T, TKey>> _field;
            private readonly IReadOnlyCollection<TKey> _allowedKeys;

            public Containment(Expression<Func<T, TKey>> field, IReadOnlyCollection<TKey> allowedKeys)
                : base(CreatePredicate(field, allowedKeys))
            {
                _field = field;
                _allowedKeys = allowedKeys;
            }

            private static Expression<Func<T, bool>> CreatePredicate(Expression<Func<T, TKey>> field, IEnumerable<TKey> allowedKeys)
            {
                // x => allowedKeys.Contains(x.field)

                var body = Expression.Call(null, Contains.Method, Expression.Constant(allowedKeys), (MemberExpression)field.Body);
                return Expression.Lambda<Func<T, bool>>(body, field.Parameters.Single());
            }

            public IReadOnlyCollection<FindSpecification<T>> SplitToBatches()
                => Enumerable.Range(0, (MsSqlBatchLimitation + _allowedKeys.Count - 1) / MsSqlBatchLimitation)
                             .Select(i => new FindSpecification<T>(CreatePredicate(_field, _allowedKeys.Skip(i * MsSqlBatchLimitation).Take(MsSqlBatchLimitation))))
                             .ToArray();
        }
    }
}