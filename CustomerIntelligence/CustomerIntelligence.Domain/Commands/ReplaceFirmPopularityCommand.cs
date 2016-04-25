using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.Replication.Core.API;

namespace NuClear.CustomerIntelligence.Domain.Commands
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