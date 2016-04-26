using NuClear.CustomerIntelligence.Replication.DTO;
using NuClear.Replication.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class ReplaceFirmCategoryForecastCommand : IReplaceDataObjectCommand
    {
        public ReplaceFirmCategoryForecastCommand(FirmForecast firmForecast)
        {
            FirmForecast = firmForecast;
        }

        public FirmForecast FirmForecast { get; }
    }
}