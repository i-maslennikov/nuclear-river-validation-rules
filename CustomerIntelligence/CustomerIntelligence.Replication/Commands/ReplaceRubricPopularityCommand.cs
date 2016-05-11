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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ReplaceRubricPopularityCommand)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DataObjectType?.GetHashCode() ?? 0) * 397) ^ RubricPopularity.GetHashCode();
            }
        }

        private bool Equals(ReplaceRubricPopularityCommand other)
        {
            return DataObjectType == other.DataObjectType && RubricPopularity.Equals(other.RubricPopularity);
        }
    }
}