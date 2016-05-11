using NuClear.Replication.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class ReplaceValueObjectCommand : IReplaceValueObjectCommand
    {
        public ReplaceValueObjectCommand(long aggregateRootId, long? entityId = null)
        {
            AggregateRootId = aggregateRootId;
            EntityId = entityId;
        }

        public long AggregateRootId { get; }
        public long? EntityId { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ReplaceValueObjectCommand)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AggregateRootId.GetHashCode() * 397) ^ EntityId.GetHashCode();
            }
        }

        protected bool Equals(ReplaceValueObjectCommand other)
        {
            return AggregateRootId == other.AggregateRootId && EntityId == other.EntityId;
        }
    }
}