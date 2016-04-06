using NuClear.Model.Common.Entities;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class RecalculateAggregatePart : IOperation
    {
        public RecalculateAggregatePart(IEntityType aggregateType, long aggregateInstanceId, IEntityType entityType, long entityInstanceId)
        {
            AggregateType = aggregateType;
            AggregateInstanceId = aggregateInstanceId;
            EntityType = entityType;
            EntityInstanceId = entityInstanceId;
        }

        public IEntityType AggregateType { get; }
        public long AggregateInstanceId { get; }
        public IEntityType EntityType { get; }
        public long EntityInstanceId { get; }

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

            return Equals((RecalculateAggregatePart)obj);
        }

        public override int GetHashCode()
        {
            var code = AggregateType.Id;
            code = (code * 397) ^ AggregateInstanceId.GetHashCode();
            code = (code * 397) ^ EntityType.Id;
            code = (code * 397) ^ EntityInstanceId.GetHashCode();
            return code;
        }

        private bool Equals(RecalculateAggregatePart other)
        {
            return AggregateType.Id == other.AggregateType.Id
                && AggregateInstanceId == other.AggregateInstanceId
                && EntityType.Id == other.EntityType.Id
                && EntityInstanceId == other.EntityInstanceId;
        }
    }
}