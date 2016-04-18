using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.Domain.Model.Statistics
{
    public sealed class ProjectStatistics : ICustomerIntelligenceAggregatePart, IIdentifiable<StatisticsKey>
    {
        public long ProjectId { get; set; }
        public long CategoryId { get; set; }
    }
}