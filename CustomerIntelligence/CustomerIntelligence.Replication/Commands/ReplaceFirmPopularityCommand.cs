using NuClear.CustomerIntelligence.Replication.DTO;
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
    }
}