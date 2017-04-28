using System;

using NuClear.Replication.Core.Commands;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.Commands
{
    public class RecalculatePeriodCommand : IAggregateCommand
    {
        public RecalculatePeriodCommand(PeriodKey periodKey)
        {
            Point = periodKey;
        }

        public PeriodKey Point { get; }
        public Type AggregateRootType => typeof(Period);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RecalculatePeriodCommand)obj);
        }

        public override int GetHashCode()
        {
            return Point.GetHashCode();
        }

        protected bool Equals(RecalculatePeriodCommand other)
        {
            return Point.Equals(other.Point);
        }
    }
}