namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IAggregateProcessor
    {
        void Initialize(AggregateProcessorSlice slice);
        void Recalculate(AggregateProcessorSlice slice);
        void Destroy(AggregateProcessorSlice slice);
    }
}
