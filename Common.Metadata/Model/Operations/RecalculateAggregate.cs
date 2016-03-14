namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class RecalculateAggregate : AggregateOperation
    {
        public RecalculateAggregate(int entityTypeId, long entityId)
            : base(entityTypeId, entityId)
        {
        }
    }
}