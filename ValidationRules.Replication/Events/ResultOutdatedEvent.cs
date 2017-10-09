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

        private bool Equals(ResultOutdatedEvent other)
        {
            return Rule == other.Rule;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ResultOutdatedEvent @event && Equals(@event);
        }

        public override int GetHashCode()
        {
            return (int)Rule;
        }

        public MessageTypeCode Rule { get; }
    }
}