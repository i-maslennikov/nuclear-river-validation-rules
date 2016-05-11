using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NuClear.CustomerIntelligence.Replication.Tests
{
    internal abstract class FixtureBase
    {
        protected static class Predicate
        {
            public static Expression<Func<T, bool>> Match<T>(T expected)
            {
                return Match(expected, x => x);
            }

            public static Expression<Func<IEnumerable<T>, bool>> SequentialMatch<T>(IEnumerable<T> expected)
            {
                return items => items.ToArray().Zip(expected, (item1, item2) => new ProjectionEqualityComparer<T, T>(x => x).Equals(item1, item2)).All(x => x);
            }

            public static Expression<Func<T, bool>> Match<T, TProjection>(T expected, Func<T, TProjection> projector)
            {
                return item => new ProjectionEqualityComparer<T, TProjection>(projector).Equals(item, expected);
            }
        }
    }
}