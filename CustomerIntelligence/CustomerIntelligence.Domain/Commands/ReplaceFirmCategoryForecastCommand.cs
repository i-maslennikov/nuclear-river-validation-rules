using NuClear.CustomerIntelligence.Replication.DTO;
using NuClear.Replication.Core;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class ReplaceFirmCategoryForecastCommand : ICommand
    {
        public ReplaceFirmCategoryForecastCommand(FirmForecast firmForecast)
        {
            FirmForecast = firmForecast;
        }

        public FirmForecast FirmForecast { get; }
    }
}