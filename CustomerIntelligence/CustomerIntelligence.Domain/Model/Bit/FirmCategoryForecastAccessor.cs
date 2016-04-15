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
    public class FirmCategoryForecastAccessor : IMemoryBasedDataObjectAccessor<FirmCategoryForecast>
    {
        public FindSpecification<FirmCategoryForecast> GetFindSpecification(ICommand command)
        {
            var replaceCommand = (ReplaceFirmCategoryForecastCommand)command;
            return Specs.Find.Bit.FirmCategoryForecast.ByBitDto(replaceCommand.FirmForecast);
        }

        public IReadOnlyCollection<FirmCategoryForecast> GetDataObjects(ICommand command)
        {
            var replaceCommand = (ReplaceFirmCategoryForecastCommand)command;
            return Specs.Map.Bit.FirmCategoryForecasts().Map(replaceCommand.FirmForecast);
        }

        public IReadOnlyCollection<IEvent> HandleChanges(IReadOnlyCollection<FirmCategoryForecast> dataObjects)
        {
            return dataObjects.Select(x => new DataObjectReplacedEvent(typeof(FirmCategoryForecast), new StatisticsKey { ProjectId = x.ProjectId, CategoryId = x.CategoryId })).ToArray();
        }
    }
}