using System.Collections.Generic;

namespace NuClear.Replication.Core
{
    public static class LinqExtensions
    {
        public static IEnumerable<IReadOnlyCollection<T>> CreateBatches<T>(this IEnumerable<T> items, int batchSize)
        {
            var buffer = new List<T>(batchSize);

            foreach (var item in items)
            {
                buffer.Add(item);

                if (buffer.Count == buffer.Capacity)
                {
                    yield return buffer;
                    buffer = new List<T>(batchSize);
                }
            }

            if (buffer.Count > 0)
            {
                yield return buffer;
            }
        }
    }
}