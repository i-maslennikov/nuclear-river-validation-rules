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

        private sealed class EqualityComparer : IEqualityComparer<ResultPartiallyOutdatedEvent>
        {
            public bool Equals(ResultPartiallyOutdatedEvent x, ResultPartiallyOutdatedEvent y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Rule == y.Rule && x.OrderIds.Equals(y.OrderIds);
            }

            public int GetHashCode(ResultPartiallyOutdatedEvent obj)
            {
                unchecked
                {
                    return ((int)obj.Rule * 397) ^ obj.OrderIds.GetHashCode();
                }
            }
        }

        public static IEqualityComparer<ResultPartiallyOutdatedEvent> Comparer { get; } = new EqualityComparer();
    }
}