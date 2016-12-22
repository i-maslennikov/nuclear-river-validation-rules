using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NuClear.ValidationRules.SingleCheck
{
    public static class Extensions
    {
        private static readonly IDictionary<Type, long> QueryDuration = new ConcurrentDictionary<Type, long>();

        public static T[] Execute<T>(this IQueryable<T> queryable)
        {
            var sw = Stopwatch.StartNew();
            var arr = queryable.ToArray();
            sw.Stop();

            long time;
            QueryDuration.TryGetValue(typeof(T), out time);
            QueryDuration[typeof(T)] = time + sw.ElapsedMilliseconds;

            return arr;
        }

        public static IReadOnlyCollection<KeyValuePair<Type, long>> Durations
            => QueryDuration.OrderByDescending(x => x.Value).ToArray();
    }
}