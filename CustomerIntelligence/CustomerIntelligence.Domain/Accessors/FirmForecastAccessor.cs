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
    public class FirmForecastAccessor : IMemoryBasedDataObjectAccessor<FirmForecast>, IDataChangesHandler<FirmForecast>
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

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmForecast> dataObjects)
            => dataObjects.Select(x => new DataObjectReplacedEvent(typeof(FirmForecast), x.ProjectId)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmForecast> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmForecast> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmForecast> dataObjects) => Array.Empty<IEvent>();
    }
}