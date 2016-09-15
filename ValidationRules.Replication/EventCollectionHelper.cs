using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.Replication
{
    internal sealed class EventCollectionHelper : IEnumerable<IEvent>
    {
        private IEnumerable<IEvent> _events = Array.Empty<IEvent>();

        public void Add<T>(Type type, IQueryable<T> queryable)
        {
            _events = _events.Concat(queryable.ToArray().Select(x => new RelatedDataObjectOutdatedEvent<T>(type, x)));
        }

        public void Add<T>(Type type, IEnumerable<T> queryable)
        {
            _events = _events.Concat(queryable.ToArray().Select(x => new RelatedDataObjectOutdatedEvent<T>(type, x)));
        }

        public IEnumerator<IEvent> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_events).GetEnumerator();
        }
    }
}