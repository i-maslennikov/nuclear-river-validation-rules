using System;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.CustomerIntelligence.Domain.Model.Bit;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class ReplaceFirmPopularityCommand : IDataObjectCommand
    {
        public ReplaceFirmPopularityCommand(FirmPopularity firmPopularity)
        {
            FirmPopularity = firmPopularity;
        }

        public FirmPopularity FirmPopularity { get; }
        public Type DataObjectType => typeof(FirmCategoryStatistics);
    }
}