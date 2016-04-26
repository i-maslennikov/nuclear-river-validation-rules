using System;

using NuClear.CustomerIntelligence.Replication.DTO;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
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
        public Type DataObjectType => typeof(ProjectCategoryStatistics);
    }
}