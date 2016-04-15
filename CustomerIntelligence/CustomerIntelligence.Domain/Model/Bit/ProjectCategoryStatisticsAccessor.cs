using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Bit
{
    public class ProjectCategoryStatisticsAccessor : IMemoryBasedDataObjectAccessor<ProjectCategoryStatistics>
    {
        public FindSpecification<ProjectCategoryStatistics> GetFindSpecification(ICommand command)
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
            return dataObjects.Select(x => new DataObjectReplacedEvent(typeof(ProjectCategoryStatistics), new StatisticsKey { ProjectId = x.ProjectId, CategoryId = x.CategoryId })).ToArray();
        }
    }
}