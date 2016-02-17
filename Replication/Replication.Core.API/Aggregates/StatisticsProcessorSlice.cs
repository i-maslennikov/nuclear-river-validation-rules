using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Aggregates
{
    public sealed class StatisticsProcessorSlice
    {
        public long ProjectId { get; set; }
        public IReadOnlyCollection<long> CategoryIds { get; set; }
    }
}