using System;

using NuClear.CustomerIntelligence.Domain.DTO;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class ReplaceFirmForecastCommand : IDataObjectCommand
    {
        public ReplaceFirmForecastCommand(FirmForecast firmForecast)
        {
            FirmForecast = firmForecast;
        }

        public FirmForecast FirmForecast { get; }
        public Type DataObjectType => typeof(Model.Bit.FirmForecast);
    }
}