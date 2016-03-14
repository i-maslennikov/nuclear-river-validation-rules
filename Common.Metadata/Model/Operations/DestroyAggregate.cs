namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class DestroyAggregate : AggregateOperation
    {
        public DestroyAggregate(int entityTypeId, long entityId)
            : base(entityTypeId, entityId)
        {
        }
    }
}