using System;

using NuClear.CustomerIntelligence.Replication.DTO;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.Replication.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class ReplaceFirmPopularityCommand : IReplaceDataObjectCommand
    {
        public ReplaceFirmPopularityCommand(FirmPopularity firmPopularity)
        {
            FirmPopularity = firmPopularity;
        }

        public FirmPopularity FirmPopularity { get; }
        public Type DataObjectType => typeof(FirmCategoryStatistics);
    }
}