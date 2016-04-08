namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class RecalculateAggregate : AggregateOperation
    {
        public RecalculateAggregate(EntityReference entityType) 
            : base(entityType)
        {
        }
    }
}