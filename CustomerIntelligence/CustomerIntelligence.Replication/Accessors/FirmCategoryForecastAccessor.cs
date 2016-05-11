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
    public class FirmCategoryForecastAccessor : IMemoryBasedDataObjectAccessor<FirmCategoryForecast>, IDataChangesHandler<FirmCategoryForecast>
    {
        public FindSpecification<FirmCategoryForecast> GetFindSpecification(ICommand command)
        {
            var replaceCommand = (ReplaceFirmCategoryForecastCommand)command;
            return new FindSpecification<FirmCategoryForecast>(x => x.ProjectId == replaceCommand.FirmForecast.ProjectId);
        }

        public IReadOnlyCollection<FirmCategoryForecast> GetDataObjects(ICommand command)
        {
            var replaceCommand = (ReplaceFirmCategoryForecastCommand)command;
            return Specs.Map.Bit.FirmCategoryForecasts().Map(replaceCommand.FirmForecast);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmCategoryForecast> dataObjects)
            => dataObjects.Select(x => new DataObjectReplacedEvent(typeof(FirmCategoryForecast), x.ProjectId)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmCategoryForecast> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmCategoryForecast> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmCategoryForecast> dataObjects) => Array.Empty<IEvent>();
    }
}