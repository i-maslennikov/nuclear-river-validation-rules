using System;

using NuClear.CustomerIntelligence.Replication.DTO;
using NuClear.Replication.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class ReplaceFirmForecastCommand : IReplaceDataObjectCommand
    {
        public ReplaceFirmForecastCommand(FirmForecast firmForecast)
        {
            FirmForecast = firmForecast;
        }

        public FirmForecast FirmForecast { get; }
        public Type DataObjectType => typeof(Storage.Model.Bit.FirmForecast);
    }
}