using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NuClear.ValidationRules.SingleCheck
{
    public static class Extensions
    {
        private static readonly IDictionary<string, long> QueryDuration = new ConcurrentDictionary<string, long>();

        public static T[] Execute<T>(this IQueryable<T> queryable, string name = null)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return queryable.ToArray();
            }
            finally
            {
                sw.Stop();
                Append(Key<T>(name), sw.ElapsedMilliseconds);
            }
        }

        private static void Append(string key, long duration)
        {
            long accumulated;
            QueryDuration.TryGetValue(key, out accumulated);
            QueryDuration[key] = accumulated + duration;
        }

        private static string Key<T>(string name)
            => string.IsNullOrEmpty(name) ? typeof(T).FullName : $"{typeof(T).FullName}, {name}";

        public static IReadOnlyCollection<KeyValuePair<string, long>> Durations
            => QueryDuration.OrderByDescending(x => x.Value).ToArray();
    }
}
