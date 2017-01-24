using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.Replication
{
    internal sealed class EventCollectionHelper<TDataObject> : IReadOnlyCollection<IEvent>
    {
        private static readonly Type DataObjectType = typeof(TDataObject);

        private readonly HashSet<IEvent> _hashSet = new HashSet<IEvent>();

        public void Add<TDataObjectId>(Type relatedDataObjectType, IEnumerable<TDataObjectId> ids)
            where TDataObjectId : struct
        {
            _hashSet.UnionWith(ids.Select(x => new RelatedDataObjectOutdatedEvent<TDataObjectId>(DataObjectType, relatedDataObjectType, x)));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IEvent> GetEnumerator() => _hashSet.GetEnumerator();

        public int Count => _hashSet.Count;
    }
}