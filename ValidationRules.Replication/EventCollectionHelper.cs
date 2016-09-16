using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.Replication
{
    internal sealed class EventCollectionHelper : IReadOnlyCollection<IEvent>
    {
        private IEnumerable<IEvent> _events = Array.Empty<IEvent>();
        private IReadOnlyCollection<IEvent> _readonlyCollection;

        public void Add<T>(Type type, IQueryable<T> queryable)
        {
            Add(type, queryable.ToArray());
        }

        public void Add<T>(Type type, IEnumerable<T> queryable)
        {
            _events = _events.Concat(queryable.ToArray().Select(x => new RelatedDataObjectOutdatedEvent<T>(type, x)));
            _readonlyCollection = null;
        }

        private IReadOnlyCollection<IEvent> ReadonlyCollection
            => _readonlyCollection ?? (_readonlyCollection = _events.ToArray());

        IEnumerator<IEvent> IEnumerable<IEvent>.GetEnumerator()
            => ReadonlyCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)ReadonlyCollection).GetEnumerator();

        int IReadOnlyCollection<IEvent>.Count
            => ReadonlyCollection.Count;
    }
}