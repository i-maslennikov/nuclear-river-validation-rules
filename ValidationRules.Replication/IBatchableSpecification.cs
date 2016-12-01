using System.Collections.Generic;

using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.Replication
{
    public interface IBatchableSpecification<T>
    {
        IReadOnlyCollection<FindSpecification<T>> SplitToBatches();
    }
}