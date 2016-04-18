using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Bit
{
    public class FirmCategoryStatisticsAccessor : IMemoryBasedDataObjectAccessor<FirmCategoryStatistics>
    {
        public FindSpecification<FirmCategoryStatistics> GetFindSpecification(ICommand command)
        {
            var replaceCommand = (ReplaceFirmPopularityCommand)command;
            return Specs.Find.Bit.FirmCategoryStatistics.ByBitDto(replaceCommand.FirmPopularity);

        }

        public IReadOnlyCollection<FirmCategoryStatistics> GetDataObjects(ICommand command)
        {
            var replaceCommand = (ReplaceFirmPopularityCommand)command;
            return Specs.Map.Bit.FirmCategoryStatistics().Map(replaceCommand.FirmPopularity);
        }

        public IReadOnlyCollection<IEvent> HandleChanges(IReadOnlyCollection<FirmCategoryStatistics> dataObjects)
        {
            return dataObjects.Select(x => new DataObjectReplacedEvent(typeof(FirmCategoryStatistics), x.ProjectId)).ToArray();
        }
    }
}