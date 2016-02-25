using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IValueObjectProcessor
    {
        void ApplyChanges(IReadOnlyCollection<long> aggregateIds);
    }
}
