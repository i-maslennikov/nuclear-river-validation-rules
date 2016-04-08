using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Bit
{
    public class FirmCategoryForecastActor : IMemoryBasedFactActor<FirmCategoryForecast>
    {
        public FindSpecification<FirmCategoryForecast> GetDataObjectsFindSpecification(ICommand command)
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
            return dataObjects.Select(x => new RecalculateStatisticsOperation(new StatisticsKey { ProjectId = x.ProjectId })).ToArray();
        }
    }
}