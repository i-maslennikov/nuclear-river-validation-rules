using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.Domain.Model.Statistics
{
    public sealed class ProjectCategoryStatistics : IIdentifiable<StatisticsKey>
    {
        public long ProjectId { get; set; }
        public long CategoryId { get; set; }
    }

    public sealed class ProjectStatistics : IIdentifiable<long>
    {
        public long Id { get; set; } // ProjectId
    }
}