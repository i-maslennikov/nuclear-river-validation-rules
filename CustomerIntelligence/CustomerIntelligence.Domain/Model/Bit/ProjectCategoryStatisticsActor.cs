using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Bit
{
    public class ProjectCategoryStatisticsActor : IMemoryBasedFactActor<ProjectCategoryStatistics>
    {
        public FindSpecification<ProjectCategoryStatistics> GetDataObjectsFindSpecification(ICommand command)
        {
            var replaceCommand = (ReplaceRubricPopularityCommand)command;
            return Specs.Find.Bit.ProjectCategoryStatistics.ByBitDto(replaceCommand.RubricPopularity);
        }

        public IReadOnlyCollection<ProjectCategoryStatistics> GetDataObjects(ICommand command)
        {
            var replaceCommand = (ReplaceRubricPopularityCommand)command;
            return Specs.Map.Bit.ProjectCategoryStatistics().Map(replaceCommand.RubricPopularity);
        }

        public IReadOnlyCollection<IEvent> HandleChanges(IReadOnlyCollection<ProjectCategoryStatistics> dataObjects)
        {
            return dataObjects.Select(x => new RecalculateStatisticsOperation(new StatisticsKey { ProjectId = x.ProjectId })).ToArray();
        }
    }
}