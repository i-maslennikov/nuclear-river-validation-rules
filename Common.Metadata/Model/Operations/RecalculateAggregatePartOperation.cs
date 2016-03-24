namespace NuClear.River.Common.Metadata.Model.Operations
{
    public class RecalculateStatisticsOperation : IOperation
    {
        
    }

    public sealed class RecalculateAggregatePart : IOperation
    {
        public RecalculateAggregatePart(int aggregateTypeId, long aggregateInstanceId, int entityTypeId, long entityInstanceId)
        {
            AggregateTypeId = aggregateTypeId;
            AggregateInstanceId = aggregateInstanceId;
            EntityTypeId = entityTypeId;
            EntityInstanceId = entityInstanceId;
        }

        public int AggregateTypeId { get; }
        public long AggregateInstanceId { get; }
        public int EntityTypeId { get; }
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
            var code = AggregateTypeId;
            code = (code * 397) ^ AggregateInstanceId.GetHashCode();
            code = (code * 397) ^ EntityTypeId;
            code = (code * 397) ^ EntityInstanceId.GetHashCode();
            return code;
        }

        private bool Equals(RecalculateAggregatePart other)
        {
            return AggregateTypeId == other.AggregateTypeId
                && AggregateInstanceId == other.AggregateInstanceId
                && EntityTypeId == other.EntityTypeId
                && EntityInstanceId == other.EntityInstanceId;
        }
    }
}