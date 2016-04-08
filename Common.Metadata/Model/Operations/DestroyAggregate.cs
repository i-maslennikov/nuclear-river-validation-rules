namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class DestroyAggregate : AggregateOperation
    {
        public DestroyAggregate(EntityReference entityType)
            : base(entityType)
        {
        }
    }
}