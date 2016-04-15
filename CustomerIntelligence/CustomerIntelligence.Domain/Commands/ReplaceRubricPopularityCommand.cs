using System;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.CustomerIntelligence.Domain.Model.Bit;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class ReplaceRubricPopularityCommand : IDataObjectCommand
    {
        public ReplaceRubricPopularityCommand(RubricPopularity rubricPopularity)
        {
            RubricPopularity = rubricPopularity;
        }

        public RubricPopularity RubricPopularity { get; }
        public Type DataObjectType => typeof(ProjectCategoryStatistics);
    }
}