using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.Domain.Model.Statistics
{
    public sealed class ProjectCategoryStatistics : IAggregatePart, IIdentifiable<StatisticsKey>
    {
        public long CategoryId { get; set; } // Не является первичным ключом, но идентификцирует сущность внутри агрегата
        public long ProjectId { get; set; }
    }

    public sealed class ProjectStatistics : IAggregateRoot
    {
        public long Id { get; set; } // ProjectId
    }
}