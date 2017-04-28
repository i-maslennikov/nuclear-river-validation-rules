using System;

using NuClear.Replication.Core.Commands;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class SyncPeriodCommand : ISyncDataObjectCommand
    {
        public SyncPeriodCommand(Type dataObjectType, DateTime date)
        {
            DataObjectType = dataObjectType;
            Date = date;
        }

        public Type DataObjectType { get; }
        public DateTime Date { get; }

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

            return Equals((SyncPeriodCommand)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DataObjectType?.GetHashCode() ?? 0) * 397) ^ Date.GetHashCode();
            }
        }

        private bool Equals(SyncPeriodCommand other)
        {
            return DataObjectType == other.DataObjectType && Date == other.Date;
        }
    }
}