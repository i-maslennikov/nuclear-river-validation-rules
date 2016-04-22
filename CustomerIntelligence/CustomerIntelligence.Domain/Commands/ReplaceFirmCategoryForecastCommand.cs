﻿using System;

﻿using NuClear.CustomerIntelligence.Domain.DTO;
﻿using NuClear.Replication.Core.API.Commands;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class ReplaceFirmCategoryForecastCommand : IDataObjectCommand
    {
        public ReplaceFirmCategoryForecastCommand(FirmForecast firmForecast)
        {
            FirmForecast = firmForecast;
        }

        public FirmForecast FirmForecast { get; }

        public Type DataObjectType => typeof(Model.Bit.FirmCategoryForecast);
    }
}