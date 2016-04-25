using System.Collections.Generic;

namespace NuClear.Replication.Core.Equality
{
    public interface IEqualityComparerFactory
    {
        IEqualityComparer<T> CreateIdentityComparer<T>();
        IEqualityComparer<T> CreateCompleteComparer<T>();
    }
}
