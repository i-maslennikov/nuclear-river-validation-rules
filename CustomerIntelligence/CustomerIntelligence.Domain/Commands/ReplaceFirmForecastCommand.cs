using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.Commands
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