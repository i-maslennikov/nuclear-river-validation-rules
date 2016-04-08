namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class InitializeAggregate : AggregateOperation
    {
        public InitializeAggregate(EntityReference entityReference)
            : base(entityReference)
        {
        }
    }
}