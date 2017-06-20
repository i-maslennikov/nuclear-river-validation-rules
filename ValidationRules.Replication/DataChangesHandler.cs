using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication
{
    public abstract class DataChangesHandler<T> : IDataChangesHandler<T>
    {
        private readonly IRuleInvalidator _invalidator;

        protected DataChangesHandler(IRuleInvalidator invalidator)
        {
            _invalidator = invalidator;
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<T> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<T> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<T> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<T> dataObjects)
            => _invalidator.Invalidate(dataObjects);

        protected interface IRuleInvalidator
        {
            IReadOnlyCollection<IEvent> Invalidate(IReadOnlyCollection<T> dataObjects);
        }

        protected sealed class RuleInvalidator : IRuleInvalidator, IEnumerable
        {
            private readonly Dictionary<MessageTypeCode, Func<IReadOnlyCollection<T>, IEvent>> _dictionary;

            public RuleInvalidator()
            {
                _dictionary = new Dictionary<MessageTypeCode, Func<IReadOnlyCollection<T>, IEvent>>();
            }

            public void Add(MessageTypeCode ruleCode)
                => _dictionary.Add(ruleCode, x => x.Any() ? new ResultOutdatedEvent(ruleCode) : null);

            public void Add(MessageTypeCode ruleCode, Func<IReadOnlyCollection<T>, IReadOnlyCollection<long>> onChange)
                => _dictionary.Add(ruleCode, x => x.Any() ? new ResultPartiallyOutdatedEvent(ruleCode, onChange.Invoke(x)) : null);

            IReadOnlyCollection<IEvent> IRuleInvalidator.Invalidate(IReadOnlyCollection<T> dataObjects)
                => _dictionary.Select(x => x.Value.Invoke(dataObjects)).Where(x => x != null).ToArray();

            IEnumerator IEnumerable.GetEnumerator()
                => ((IEnumerable)_dictionary).GetEnumerator();
        }
    }
}
