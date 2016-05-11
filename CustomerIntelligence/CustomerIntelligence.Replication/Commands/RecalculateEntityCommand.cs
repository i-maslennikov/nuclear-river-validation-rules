using System;

using NuClear.Replication.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class RecalculateEntityCommand : IAggregateCommand
    {
        public RecalculateEntityCommand(Type aggregateRootType, long aggregateRootId, Type entityType, long entityId)
        {
            AggregateRootType = aggregateRootType;
            AggregateRootId = aggregateRootId;
            EntityType = entityType;
            EntityId = entityId;
        }

        public Type AggregateRootType { get; }
        public long AggregateRootId { get; }

        public Type EntityType { get; }
        public long EntityId { get; }

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

            return Equals((RecalculateEntityCommand)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((AggregateRootType?.GetHashCode() ?? 0) * 397) ^ AggregateRootId.GetHashCode() ^
                       ((EntityType?.GetHashCode() ?? 0) * 397) ^ EntityId.GetHashCode();
            }
        }

        private bool Equals(RecalculateEntityCommand other)
        {
            return AggregateRootType == other.AggregateRootType && AggregateRootId == other.AggregateRootId &&
                   EntityType == other.EntityType && EntityId == other.EntityId;
        }
    }
}