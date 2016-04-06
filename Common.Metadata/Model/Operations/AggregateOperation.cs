using System;

using NuClear.Model.Common.Entities;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public abstract class AggregateOperation : IOperation
    {
        protected AggregateOperation(IEntityType entityType, long entityId)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            EntityType = entityType;
            EntityId = entityId;
        }

        public IEntityType EntityType { get; }

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
            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((AggregateOperation)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EntityType.Id * 397) ^ EntityId.GetHashCode();
            }
        }

        private bool Equals(AggregateOperation other)
        {
            return EntityType.Id == other.EntityType.Id && EntityId == other.EntityId;
        }

        public override string ToString()
        {
            return $"{GetType().Name}({EntityType.Id}, {EntityId})";
        }
    }
}