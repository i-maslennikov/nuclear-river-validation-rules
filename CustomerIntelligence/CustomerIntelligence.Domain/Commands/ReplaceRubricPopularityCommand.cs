using NuClear.CustomerIntelligence.Replication.DTO;
using NuClear.Replication.Core;

namespace NuClear.CustomerIntelligence.Replication.Commands
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