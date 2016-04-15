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
    public class FirmForecastAccessor : IMemoryBasedDataObjectAccessor<FirmForecast>
    {
        public FindSpecification<FirmForecast> GetFindSpecification(ICommand command)
        {
            var replaceCommand = (ReplaceFirmForecastCommand)command;
            return Specs.Find.Bit.FirmForecast.ByBitDto(replaceCommand.FirmForecast);
        }

        public IReadOnlyCollection<FirmForecast> GetDataObjects(ICommand command)
        {
            var replaceCommand = (ReplaceFirmForecastCommand)command;
            return Specs.Map.Bit.FirmForecasts().Map(replaceCommand.FirmForecast);
        }

        public IReadOnlyCollection<IEvent> HandleChanges(IReadOnlyCollection<FirmForecast> dataObjects)
        {
            return dataObjects.Select(x => new DataObjectReplacedEvent(typeof(FirmForecast), new StatisticsKey { ProjectId = x.ProjectId })).ToArray();
        }
    }
}