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
    public class FirmCategoryStatisticsAccessor : IMemoryBasedDataObjectAccessor<FirmCategoryStatistics>, IDataChangesHandler<FirmCategoryStatistics>
    {
        public FindSpecification<FirmCategoryStatistics> GetFindSpecification(ICommand command)
        {
            var replaceCommand = (ReplaceFirmPopularityCommand)command;
            return new FindSpecification<FirmCategoryStatistics>(x => x.ProjectId == replaceCommand.FirmPopularity.ProjectId);
        }

        public IReadOnlyCollection<FirmCategoryStatistics> GetDataObjects(ICommand command)
        {
            var replaceCommand = (ReplaceFirmPopularityCommand)command;
            return Specs.Map.Bit.FirmCategoryStatistics().Map(replaceCommand.FirmPopularity);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmCategoryStatistics> dataObjects)
            => dataObjects.Select(x => new DataObjectReplacedEvent(typeof(FirmCategoryStatistics), x.ProjectId)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmCategoryStatistics> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmCategoryStatistics> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmCategoryStatistics> dataObjects) => Array.Empty<IEvent>();
    }
}