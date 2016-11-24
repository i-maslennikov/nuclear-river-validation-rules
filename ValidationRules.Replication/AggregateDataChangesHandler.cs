using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication
{
    public abstract class AggregateDataChangesHandler<T> : IDataChangesHandler<T>
    {
        private readonly Dictionary<MessageTypeCode, Func<IReadOnlyCollection<T>, IEvent>> _dictionary;

        protected AggregateDataChangesHandler()
        {
            _dictionary = new Dictionary<MessageTypeCode, Func<IReadOnlyCollection<T>, IEvent>>();
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<T> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<T> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<T> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<T> dataObjects)
            => _dictionary.Select(x => x.Value.Invoke(dataObjects)).Where(x => x != null).ToArray();

        protected void Invalidate(MessageTypeCode ruleCode)
            => _dictionary[ruleCode] = x => x.Any() ? new ResultOutdatedEvent(ruleCode) : null;

        protected void Invalidate(MessageTypeCode ruleCode, Func<IReadOnlyCollection<T>, IReadOnlyCollection<long>> onChange)
            => _dictionary[ruleCode] = x => x.Any() ? new ResultPartiallyOutdatedEvent(ruleCode, onChange.Invoke(x)) : null;
    }
}
