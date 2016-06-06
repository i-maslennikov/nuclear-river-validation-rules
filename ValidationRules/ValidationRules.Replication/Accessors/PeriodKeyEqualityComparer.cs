using System.Collections.Generic;

namespace NuClear.ValidationRules.Replication.Accessors
{
    internal sealed class PeriodKeyEqualityComparer : IEqualityComparer<PeriodKey>
    {
        public static IEqualityComparer<PeriodKey> Instance = new PeriodKeyEqualityComparer();

        private PeriodKeyEqualityComparer() { }

        public bool Equals(PeriodKey x, PeriodKey y)
        {
            return x.ProjectId == y.ProjectId && x.Start == y.Start;
        }

        public int GetHashCode(PeriodKey obj)
        {
            return obj.ProjectId.GetHashCode() ^ obj.Start.GetHashCode();
        }
    }
}