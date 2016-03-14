namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class InitializeAggregate : AggregateOperation
    {
        public InitializeAggregate(int entityTypeId, long entityId)
            : base(entityTypeId, entityId)
        {
        }
    }
}