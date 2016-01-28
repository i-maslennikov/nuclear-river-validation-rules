using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Aggregates
{
    public sealed class AggregateProcessorSlice
    {
        public int AggregateTypeId { get; set; }
        public IReadOnlyCollection<long> AggregateIds { get; set; }
    }
}