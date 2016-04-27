using System;
using System.Collections.Generic;
using System.Linq;

namespace NuClear.CustomerIntelligence.Replication.Tests
{
    public static class QueryableExtensions
    {
        public static IEnumerable<T> By<T>(this IQueryable<T> queryable, Func<T, long> idProvider, long id)
        {
            return queryable.Where(x => idProvider(x) == id);
        }

        public static IEnumerable<T> By<T>(this IQueryable<T> queryable, Func<T, long> idProvider, IEnumerable<long> ids)
        {
            return queryable.Where(x => ids.Contains(idProvider(x)));
        }
    }
}