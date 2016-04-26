using NuClear.CustomerIntelligence.Replication.DTO;
using NuClear.Replication.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class ReplaceRubricPopularityCommand : IReplaceDataObjectCommand
    {
        public ReplaceRubricPopularityCommand(RubricPopularity rubricPopularity)
        {
            RubricPopularity = rubricPopularity;
        }

        public RubricPopularity RubricPopularity { get; }
    }
}