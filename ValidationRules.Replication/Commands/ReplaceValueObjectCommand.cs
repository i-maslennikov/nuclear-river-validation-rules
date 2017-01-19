using NuClear.Replication.Core.Commands;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class ReplaceValueObjectCommand : IReplaceValueObjectCommand
    {
        public ReplaceValueObjectCommand(long aggregateRootId, long? entityId = null)
        {
            AggregateRootId = aggregateRootId;
            EntityId = entityId;
        }

        public long AggregateRootId { get; }
        public long? EntityId { get; }

        private bool Equals(ReplaceValueObjectCommand other)
        {
            return AggregateRootId == other.AggregateRootId && EntityId == other.EntityId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var a = obj as ReplaceValueObjectCommand;
            return a != null && Equals(a);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AggregateRootId.GetHashCode() * 397) ^ EntityId.GetHashCode();
            }
        }
    }
}