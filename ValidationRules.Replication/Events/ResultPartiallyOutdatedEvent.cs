using System.Collections.Generic;

using NuClear.Replication.Core;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.Events
{
    public sealed class ResultPartiallyOutdatedEvent : IEvent
    {
        public ResultPartiallyOutdatedEvent(MessageTypeCode rule, IReadOnlyCollection<long> orderIds)
        {
            Rule = rule;
            OrderIds = orderIds;
        }

        public MessageTypeCode Rule { get; }
        public IReadOnlyCollection<long> OrderIds { get; }

        private bool Equals(ResultPartiallyOutdatedEvent other)
        {
            return Rule == other.Rule && OrderIds.Equals(other.OrderIds);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ResultPartiallyOutdatedEvent @event && Equals(@event);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Rule * 397) ^ OrderIds.GetHashCode();
            }
        }
    }
}