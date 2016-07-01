using System;

using NuClear.Replication.Core.Commands;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class RecalculatePeriodAggregateCommand : IAggregateCommand
    {
        public Type AggregateRootType => typeof(Period);

        public RecalculatePeriodAggregateCommand(PeriodKey periodKey)
        {
            PeriodKey = periodKey;
        }

        public PeriodKey PeriodKey { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is RecalculatePeriodAggregateCommand && Equals((RecalculatePeriodAggregateCommand)obj);
        }

        private bool Equals(RecalculatePeriodAggregateCommand other)
        {
            return PeriodKey.Equals(other.PeriodKey);
        }

        public override int GetHashCode()
        {
            return PeriodKey.GetHashCode();
        }
    }

    public sealed class SyncPeriodDataObjectCommand : ISyncDataObjectCommand
    {
        public Type DataObjectType => typeof(Period);

        public SyncPeriodDataObjectCommand(PeriodKey periodKey)
        {
            PeriodKey = periodKey;
        }

        public PeriodKey PeriodKey { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is SyncPeriodDataObjectCommand && Equals((SyncPeriodDataObjectCommand)obj);
        }

        private bool Equals(SyncPeriodDataObjectCommand other)
        {
            return PeriodKey.Equals(other.PeriodKey);
        }

        public override int GetHashCode()
        {
            return PeriodKey.GetHashCode();
        }
    }

    public sealed class ReplacePeriodValueObjectCommand : IReplaceValueObjectCommand
    {
        public ReplacePeriodValueObjectCommand(PeriodKey periodKey)
        {
            PeriodKey = periodKey;
        }

        public PeriodKey PeriodKey { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ReplacePeriodValueObjectCommand && Equals((ReplacePeriodValueObjectCommand)obj);
        }

        private bool Equals(ReplacePeriodValueObjectCommand other)
        {
            return PeriodKey.Equals(other.PeriodKey);
        }

        public override int GetHashCode()
        {
            return PeriodKey.GetHashCode();
        }
    }
}