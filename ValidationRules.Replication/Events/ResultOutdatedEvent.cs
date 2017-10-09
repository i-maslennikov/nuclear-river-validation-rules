using System.Collections.Generic;

using NuClear.Replication.Core;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.Events
{
    public sealed class ResultOutdatedEvent : IEvent
    {
        public ResultOutdatedEvent(MessageTypeCode rule)
        {
            Rule = rule;
        }

        public MessageTypeCode Rule { get; }

        private sealed class EqualityComparer : IEqualityComparer<ResultOutdatedEvent>
        {
            public bool Equals(ResultOutdatedEvent x, ResultOutdatedEvent y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Rule == y.Rule;
            }

            public int GetHashCode(ResultOutdatedEvent obj)
            {
                return (int)obj.Rule;
            }
        }

        public static IEqualityComparer<ResultOutdatedEvent> Comparer { get; } = new EqualityComparer();
    }
}