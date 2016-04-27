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

            return Equals((ReplaceFirmPopularityCommand)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DataObjectType?.GetHashCode() ?? 0) * 397) ^ FirmPopularity.GetHashCode();
            }
        }

        private bool Equals(ReplaceFirmPopularityCommand other)
        {
            return DataObjectType == other.DataObjectType && FirmPopularity.Equals(other.FirmPopularity);
        }
    }
}