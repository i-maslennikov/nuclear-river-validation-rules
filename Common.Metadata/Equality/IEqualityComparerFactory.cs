using System.Collections.Generic;

namespace NuClear.River.Common.Metadata.Equality
{
    public interface IEqualityComparerFactory
    {
        IEqualityComparer<T> CreateIdentityComparer<T>();
        IEqualityComparer<T> CreateCompleteComparer<T>();
    }
}
