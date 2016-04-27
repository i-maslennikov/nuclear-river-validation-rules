using System;

using NuClear.CustomerIntelligence.Replication.DTO;
using NuClear.Replication.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class ReplaceFirmCategoryForecastCommand : IReplaceDataObjectCommand
    {
        public ReplaceFirmCategoryForecastCommand(FirmForecast firmForecast)
        {
            FirmForecast = firmForecast;
        }

        public FirmForecast FirmForecast { get; }
        public Type DataObjectType => typeof(Storage.Model.Bit.FirmCategoryForecast);

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

            return Equals((ReplaceFirmCategoryForecastCommand)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DataObjectType?.GetHashCode() ?? 0) * 397) ^ FirmForecast.GetHashCode();
            }
        }

        private bool Equals(ReplaceFirmCategoryForecastCommand other)
        {
            return DataObjectType == other.DataObjectType && FirmForecast.Equals(other.FirmForecast);
        }
    }
}