namespace NuClear.River.Common.Metadata.Model.Operations
{
    public abstract class AggregateOperation : IOperation
    {
        protected AggregateOperation(int entityTypeId, long entityId)
        {
            EntityTypeId = entityTypeId;
            EntityId = entityId;
        }

        public int EntityTypeId { get; }

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
                return (EntityTypeId * 397) ^ EntityId.GetHashCode();
            }
        }

        private bool Equals(AggregateOperation other)
        {
            return EntityTypeId == other.EntityTypeId && EntityId == other.EntityId;
        }

        public override string ToString()
        {
            return $"{GetType().Name}({EntityTypeId}, {EntityId})";
        }
    }
}