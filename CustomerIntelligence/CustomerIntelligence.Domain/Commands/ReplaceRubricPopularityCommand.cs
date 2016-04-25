using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.Replication.Core.API;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class ReplaceRubricPopularityCommand : ICommand
    {
        public ReplaceRubricPopularityCommand(RubricPopularity rubricPopularity)
        {
            RubricPopularity = rubricPopularity;
        }

        public RubricPopularity RubricPopularity { get; }
    }
}