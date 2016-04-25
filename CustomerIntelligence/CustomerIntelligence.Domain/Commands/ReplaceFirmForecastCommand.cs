using NuClear.CustomerIntelligence.Replication.DTO;
using NuClear.Replication.Core;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class ReplaceFirmForecastCommand : ICommand
    {
        public ReplaceFirmForecastCommand(FirmForecast firmForecast)
        {
            FirmForecast = firmForecast;
        }

        public FirmForecast FirmForecast { get; }
    }
}