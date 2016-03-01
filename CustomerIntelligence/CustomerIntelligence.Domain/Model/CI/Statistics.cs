using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class Statistics : IIdentifiable<StatisticsKey>
    {
        public long ProjectId { get; set; }
        public long CategoryId { get; set; }
    }
}