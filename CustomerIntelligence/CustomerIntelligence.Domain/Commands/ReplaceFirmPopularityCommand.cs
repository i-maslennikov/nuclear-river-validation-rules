using NuClear.CustomerIntelligence.Replication.DTO;
using NuClear.Replication.Core;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class ReplaceFirmPopularityCommand : ICommand
    {
        public ReplaceFirmPopularityCommand(FirmPopularity firmPopularity)
        {
            FirmPopularity = firmPopularity;
        }

        public FirmPopularity FirmPopularity { get; }
    }
}