using System.Collections.Generic;
using System.Linq;

namespace ValidationRules.Replication.DatabaseComparison.Tests
{
    public sealed class EntityChangesDto
    {
        public IEnumerable<object> SourceOnly { get; set; }
        public IEnumerable<object> DestOnly { get; set; }
        public IEnumerable<object> SourceChanged { get; set; }
        public IEnumerable<object> DestChanged { get; set; }
    }

    public sealed class EntityChanges<T>
    {
        private readonly HashSet<T> _source;
        private readonly HashSet<T> _dest;

        public EntityChanges(IEnumerable<T> source, IEnumerable<T> dest, IEqualityComparer<T> comparer)
        {
            _source = new HashSet<T>(source, comparer);
            _dest = new HashSet<T>(dest, comparer);
        }

        public IEnumerable<T> SourceOnly => _source.Where(x => !_dest.Contains(x));
        public IEnumerable<T> DestOnly => _dest.Where(x => !_source.Contains(x));
        public IEnumerable<T> SourceChanged => _source.Where(x => _dest.Contains(x));
        public IEnumerable<T> DestChanged => _dest.Where(x => _source.Contains(x));
    }
}