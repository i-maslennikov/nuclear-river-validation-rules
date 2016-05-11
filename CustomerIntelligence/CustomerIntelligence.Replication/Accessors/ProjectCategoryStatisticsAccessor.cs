using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Accessors
{
    public class ProjectCategoryStatisticsAccessor : IMemoryBasedDataObjectAccessor<ProjectCategoryStatistics>, IDataChangesHandler<ProjectCategoryStatistics>
    {
        public FindSpecification<ProjectCategoryStatistics> GetFindSpecification(ICommand command)
        {
            var replaceCommand = (ReplaceRubricPopularityCommand)command;
            return new FindSpecification<ProjectCategoryStatistics>(x => x.ProjectId == replaceCommand.RubricPopularity.ProjectId);
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