using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class ReplaceFirmCategoryForecastCommand : ICommand
    {
        public ReplaceFirmCategoryForecastCommand(FirmForecast firmForecast)
        {
            FirmForecast = firmForecast;
        }

        public FirmForecast FirmForecast { get; set; }
    }
}