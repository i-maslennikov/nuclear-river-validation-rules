using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Statistics
{
    public sealed class ProjectStatistics : IAggregateRoot
    {
        public long Id { get; set; } // ProjectId
    }
}