using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Model.Bit;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Accessors
{
    public class ProjectCategoryStatisticsAccessor : IMemoryBasedDataObjectAccessor<ProjectCategoryStatistics>, IDataChangesHandler<ProjectCategoryStatistics>
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

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<ProjectCategoryStatistics> dataObjects)
            => dataObjects.Select(x => new DataObjectReplacedEvent(typeof(ProjectCategoryStatistics), x.ProjectId)).ToArray();


        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<ProjectCategoryStatistics> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<ProjectCategoryStatistics> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<ProjectCategoryStatistics> dataObjects) => Array.Empty<IEvent>();
    }
}