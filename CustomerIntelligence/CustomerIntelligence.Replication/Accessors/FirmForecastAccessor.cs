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
    public class FirmForecastAccessor : IMemoryBasedDataObjectAccessor<FirmForecast>, IDataChangesHandler<FirmForecast>
    {
        public FindSpecification<FirmForecast> GetFindSpecification(ICommand command)
        {
            var replaceCommand = (ReplaceFirmForecastCommand)command;
            return new FindSpecification<FirmForecast>(x => x.ProjectId == replaceCommand.FirmForecast.ProjectId);
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